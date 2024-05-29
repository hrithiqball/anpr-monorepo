using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Http;
using MlffSiteService.Extensions;
using MlffSiteService.Interface;
using MlffSiteService.Models;
using MlffSiteService.Simulation;
using ENV_KEY = MlffSiteService.Models.Constants.EnvironmentVariables;
using Timer = System.Timers.Timer;

namespace MlffSiteService.Services;

[SuppressMessage("Usage", "CA2254:Template should be a static expression")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation.Evident")]
public class MasterModeratorService : IMasterModeratorService
{
    private readonly ILogger<MasterModeratorService> _logger;
    private readonly CancellationTokenSource _cancellationToken;

    private readonly IAnprService _anprService;
    private readonly IRfidReaderService _rfidReaderService;
    private readonly ISpeedRadarService _speedRadarService;

    private readonly IMlffWebApiManager _mlffWebApiManager;

    private IList<ISpeedDetectionResult> _speedDetectionList;
    private IList<IRfidTagDetectionResult> _rfidTagDetectionList;
    private IList<IAnprDetectionResult> _anprDetectionList;

    private readonly IList<TimeoutTask> _timeoutTasks;

    private readonly double _searchWindowInMilliseconds = 500;
    private readonly bool _enableSimulation;
    private readonly bool _enablePostAnpr;
    private readonly bool _enablePostRfid;
    private readonly bool _enablePostSpeed;
    private readonly bool _enableLane1;
    private readonly bool _enableLane2;
    private readonly bool _presenceRadarLane1;
    private readonly bool _presenceRadarLane2;

    private string dynamicIp = "";
    private string prevRfid = "";

    private const string TRIGGER_BY_ANPR = "ANPR";
    private const string TRIGGER_BY_SPEED = "SPEED";
    private const string TRIGGER_BY_RFID = "RFID";
    private const string TIMESTAMP_FORMAT_WITH_MILLISECONDS = "dd/MM/yyyy HH:mm:ss.fff";
    private const int SIMULATION_DELAY_MIN_MILLISECONDS = 200;
    private const int SIMULATION_DELAY_MAX_MILLISECONDS = 5000;

    public MasterModeratorService(
        ILogger<MasterModeratorService> logger,
        ISpeedRadarService speedRadarService,
        IAnprService anprService,
        IRfidReaderService rfidReaderService,
        IMlffWebApiManager mlffWebApiManager
    )
    {
        _logger = logger;
        _cancellationToken = new CancellationTokenSource();
        _speedRadarService = speedRadarService;
        _anprService = anprService;
        _rfidReaderService = rfidReaderService;
        _mlffWebApiManager = mlffWebApiManager;
        _speedDetectionList = new List<ISpeedDetectionResult>();
        _rfidTagDetectionList = new List<IRfidTagDetectionResult>();
        _anprDetectionList = new List<IAnprDetectionResult>();
        _timeoutTasks = new List<TimeoutTask>();

        var enablePostAnpr = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_POST_ANPR);
        if (!string.IsNullOrEmpty(enablePostAnpr))
        {
            if (!bool.TryParse(enablePostAnpr, out _enablePostAnpr))
            {
                _enablePostAnpr = true;
            }
        }

        var enablePostSpeed = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_POST_SPEED);
        if (!string.IsNullOrEmpty(enablePostSpeed))
        {
            if (!bool.TryParse(enablePostSpeed, out _enablePostSpeed))
            {
                _enablePostSpeed = false;
            }
        }

        var enablePostRfid = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_POST_RFID);
        if (!string.IsNullOrEmpty(enablePostRfid))
        {
            if (!bool.TryParse(enablePostRfid, out _enablePostSpeed))
            {
                _enablePostRfid = false;
            }
        }

        var enableLane1 = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_LANE_1);
        if (!string.IsNullOrEmpty(enableLane1))
        {
            if (!bool.TryParse(enableLane1, out _enableLane1))
            {
                _enableLane1 = false;
            }
        }

        var enableLane2 = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_LANE_2);
        if (!string.IsNullOrEmpty(enableLane2))
        {
            if (!bool.TryParse(enableLane2, out _enableLane2))
            {
                _enableLane2 = false;
            }
        }

        var presenceRadarLane1 = Environment.GetEnvironmentVariable(ENV_KEY.PRESENCE_RADAR_LANE_1);
        if (!string.IsNullOrEmpty(presenceRadarLane1))
        {
            if (!bool.TryParse(enableLane2, out _presenceRadarLane1))
            {
                _presenceRadarLane1 = false;
            }
        }

        var presenceRadarLane2 = Environment.GetEnvironmentVariable(ENV_KEY.PRESENCE_RADAR_LANE_2);
        if (!string.IsNullOrEmpty(presenceRadarLane1))
        {
            if (!bool.TryParse(enableLane2, out _presenceRadarLane2))
            {
                _presenceRadarLane2 = false;
            }
        }

        var enableSimulation = Environment.GetEnvironmentVariable(ENV_KEY.ENABLE_SIMULATION);
        if (!string.IsNullOrEmpty(enableSimulation))
        {
            if (!bool.TryParse(enableSimulation, out _enableSimulation))
            {
                _enableSimulation = false;
            }
        }

        var searchWindow = Environment.GetEnvironmentVariable(
            ENV_KEY.SEARCH_WINDOW_IN_MILLISECONDS
        );
        if (!string.IsNullOrEmpty(searchWindow))
        {
            _searchWindowInMilliseconds = double.Parse(searchWindow);
        }

        Task ClearCacheList = Task.Run(() =>
        {
            while (true)
            {
                DisposeCacheListDetection();
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = client.GetAsync("http://api.ipify.org").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string tempIP = response.Content.ReadAsStringAsync().Result;
                            if (tempIP != dynamicIp)
                            {
                                dynamicIp = tempIP;
                                RegisterPublicIPEventHandler(dynamicIp);
                            }
                        }
                        else
                        {
                            _logger.LogError("ipify server down");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        dynamicIp = "Error IP";
                    }
                }
                Thread.Sleep(60000);
            }
        });

        RegisterDetectionEventHandler();
    }

    public void Dispose()
    {
        _cancellationToken.Dispose();

        _anprService.OnAnprDetectionResult -= SendToMlffWebApi_Anpr;
        _speedRadarService.OnSpeedDetected -= SendToMlffWebApi_Speed;
        _rfidReaderService.OnRfidTagDetectionResult -= SendToMlffWebApi_Rfid;

        _anprService.OnAnprDetectionResult -= InsertToMatchingList_Anpr;
        _speedRadarService.OnSpeedDetected -= InsertToMatchingList_Speed;
        _rfidReaderService.OnRfidTagDetectionResult -= InsertToMatchingList_Rfid;
    }

    public async Task StartAsync()
    {
        await _anprService.StartAsync();
        await _speedRadarService.StartAsync();
        await _rfidReaderService.StartAsync();

        if (_enableSimulation)
        {
            StartSimulate();
        }
    }

    private void DisposeCacheListDetection()
    {
        DateTime currentTime = DateTime.Now;

        if (currentTime.Minute % 5 == 0)
        {
            DisposeCache(currentTime);
        }
        else
        {
            _logger.LogCritical(_anprDetectionList.Count.ToString() + " Anpr cache list");
            _logger.LogCritical(_rfidTagDetectionList.Count.ToString() + " Rfid cache list");
            _logger.LogCritical(_speedDetectionList.Count.ToString() + " Speed cache list");
        }
    }

    private void DisposeCache(DateTime now)
    {
        DateTime cutoff = new(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        if (_anprDetectionList != null)
        {
            _anprDetectionList = _anprDetectionList
                .Where(t => t.Timestamp >= cutoff.AddMinutes(-1))
                .ToList();
            _anprDetectionList.Clear();
            _logger.LogCritical("Anpr count is: " + _anprDetectionList.Count.ToString());
        }
        if (_rfidTagDetectionList != null)
        {
            _rfidTagDetectionList = _rfidTagDetectionList
                .Where(t => t.Timestamp >= cutoff.AddMinutes(-1))
                .ToList();
            _rfidTagDetectionList.Clear();
            _logger.LogCritical("Rfid count is: " + _rfidTagDetectionList.Count.ToString());
        }
        if (_speedDetectionList != null)
        {
            _speedDetectionList = _speedDetectionList
                .Where(t => t.Timestamp >= cutoff.AddMinutes(-1))
                .ToList();
            _speedDetectionList.Clear();
            _logger.LogCritical("Speed count is: " + _speedDetectionList.Count.ToString());
        }
    }

    private void RegisterDetectionEventHandler()
    {
        // separate out the matching with send

        _anprService.OnAnprDetectionResult += InsertToMatchingList_Anpr;
        _speedRadarService.OnSpeedDetected += InsertToMatchingList_Speed;
        _rfidReaderService.OnRfidTagDetectionResult += InsertToMatchingList_Rfid;

        if (_enablePostSpeed)
        {
            _speedRadarService.OnSpeedDetected += SendToMlffWebApi_Speed;
        }
        if (_enablePostAnpr)
        {
            _anprService.OnAnprDetectionResult += SendToMlffWebApi_Anpr;
        }
        if (_enablePostRfid)
        {
            _rfidReaderService.OnRfidTagDetectionResult += SendToMlffWebApi_Rfid;
        }
    }

    #region ipaddress
    private void RegisterPublicIPEventHandler(string dynamicIP)
    {
        SendToMlffWebApi_PublicIPAsync(dynamicIP).ConfigureAwait(false);
    }

    private async Task SendToMlffWebApi_PublicIPAsync(string dynamicIPNew)
    {
        string publicIPString = dynamicIPNew;
        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostPublicIPAsync(
                publicIPString,
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                _logger.LogError(
                    "Unable to submit public IP address to web api. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit public IP address result");
        }
    }
    #endregion

    #region anpr

    private void SendToMlffWebApi_Anpr(IAnprDetectionResult anpr)
    {
        switch (_anprService.Config.AnprEngineType)
        {
            case Constants.AnprEngineTypes.RECOANPR:
#pragma warning disable CS8604 // Possible null reference argument.
                SendToMlffWebApi_RecoANPR(anpr as AnprDetectionResultRecoAnpr);
#pragma warning restore CS8604 // Possible null reference argument.
                break;
            default:
                _logger.LogWarning("Unknown engine type");
                break;
        }
    }

    private void InsertToMatchingList_Anpr(IAnprDetectionResult anpr)
    {
        InsertAnprDetectionToWatchingList(anpr as AnprDetectionResultRecoAnpr);
        //InsertAnprDetectionToWatchingList((AnprDetectionResultRecoAnpr)anpr);

        TryToMatchTimeoutByConstant(anpr);

        var logFolder = Environment.GetEnvironmentVariable(ENV_KEY.LOG_OUTPUT_PATH);

        var anprDetectionCsvLogDirectory = Path.Join(
            logFolder,
            $"anpr_detection_{DateTime.Now:yyyyMMdd}.csv"
        );
        //string cameraId = (anpr as AnprDetectionResultRecoAnpr)?.CameraId ?? string.Empty;
        string[] columns = new string[] { "Timestamp", "Camera ID", "Plate Number" };
        string[] values = new string[]
        {
            anpr.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS),
            anpr.CameraId,
            anpr.PlateNumber,
        };
        WriteCsvLog(anprDetectionCsvLogDirectory, columns, values);

        //var anprDetectionLogDirectory = Path.Join(logFolder, $"anpr_detection_{DateTime.Now:yyyyMMdd}.log");
        //WriteTextLog(anprDetectionLogDirectory, $"{anpr.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS),25} | {"ANPR",6} | {anpr.Serialize(false)}");
    }

    private void SendToMlffWebApi_RecoANPR(AnprDetectionResultRecoAnpr anpr)
    {
        SendToMlffWebApi_AnprAsync(anpr).ConfigureAwait(false);
    }

    private async Task SendToMlffWebApi_AnprAsync(AnprDetectionResultRecoAnpr anpr)
    {
        if (!_enableSimulation)
        // if (_enableSimulation)
        {
            // get image data
            var imageNetworkPath = _anprService.Config.ImageMountedNetworkPath;
            var vehicleImageFilePath = Path.GetFullPath(anpr.VehicleImagePath, imageNetworkPath);
            var plateImageFilePath = Path.GetFullPath(anpr.PlateImagePath, imageNetworkPath);
            var vehicleImageFileInfo = new FileInfo(vehicleImageFilePath);
            var plateImageFileInfo = new FileInfo(plateImageFilePath);

            try
            {
                var uploadImageResponseBody = await _mlffWebApiManager.UploadImageAsync(
                    vehicleImageFileInfo,
                    plateImageFileInfo,
                    anpr.ImageId,
                    anpr.PlateNumber,
                    _cancellationToken.Token
                );

                if (uploadImageResponseBody.StatusCode != StatusCodes.Status200OK)
                {
                    // failed to upload
                    _logger.LogError(
                        "Unable to upload ANPR images. Response payload: {ResponseMessage}",
                        uploadImageResponseBody.Serialize()
                    );
                    // TODO: insert to database as back log data
                    return;
                }

                anpr.PlateImagePath = uploadImageResponseBody.Data.PlateImageUrl;
                anpr.VehicleImagePath = uploadImageResponseBody.Data.VehicleImageUrl;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to upload ANPR images");
                // TODO: insert to database as back log data
                return;
            }
        }

        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostRecoAnprResultAsync(
                anpr.ToArray(),
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                // failed to submit anpr result
                _logger.LogError(
                    "Unable to submit ANPR result. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
                // TODO: insert to database as back log data
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit ANPR result");
            // TODO: insert to database as back log data
        }
    }

    private void InsertAnprDetectionToWatchingList(AnprDetectionResultRecoAnpr anpr)
    {
        _anprDetectionList.Add(anpr);

        // remove staled detection from list
        _anprDetectionList = _anprDetectionList
            .Except(_anprDetectionList.Where(t => t.Timestamp > DateTime.Now.AddSeconds(10)))
            .ToList();
    }

    #endregion anpr

    #region speed

    private void InsertToMatchingList_Speed(ISpeedDetectionResult speedDetection)
    {
        InsertSpeedDetectionToWatchingList(speedDetection);
        //TryToStartTimeoutTask(null, speedDetection, null);

        var logFolder = Environment.GetEnvironmentVariable(ENV_KEY.LOG_OUTPUT_PATH);

        //csv logs
        var speedDetectionCsvLogDirectory = Path.Join(
            logFolder,
            $"speed_detection_{DateTime.Now:yyyyMMdd}.csv"
        );
        string[] columns = new string[] { "Timestamp", "Speed" };
        string[] values = new string[]
        {
            speedDetection.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS),
            speedDetection.Speed.ToString()
        };
        WriteCsvLog(speedDetectionCsvLogDirectory, columns, values);

        //WriteTextLog(speedDetectionLogDirectory, $"{speedDetection.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS),25} | {"SPEED",6} | {speedDetection.Serialize(false)}\n");
    }

    private void WriteCsvLog(string directory, string[] columns, string[] values)
    {
        string header = string.Join(",", columns);
        string row = string.Join(",", values);

        if (!File.Exists(directory))
        {
            using (StreamWriter writer = new StreamWriter(directory, append: true, Encoding.UTF8))
            {
                writer.WriteLine(header);
            }
        }

        using (StreamWriter writer = new StreamWriter(directory, append: true, Encoding.UTF8))
        {
            writer.WriteLine(row);
        }
    }

    private void SendToMlffWebApi_Speed(ISpeedDetectionResult speedDetection)
    {
        SendToMlffWebApi_SpeedAsync(speedDetection).ConfigureAwait(false);
    }

    private async Task SendToMlffWebApi_SpeedAsync(ISpeedDetectionResult speedDetection)
    {
        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostSpeedDetectionAsync(
                speedDetection.ToArray(),
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                // failed to submit anpr result
                _logger.LogError(
                    "Unable to submit speed detection result. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
                // TODO: insert to database as back log data
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit speed detection result");
            // TODO: insert to database as back log data
        }
    }

    private void InsertSpeedDetectionToWatchingList(ISpeedDetectionResult speedDetection)
    {
        _speedDetectionList.Add(speedDetection);

        // remove staled detection from list
        _speedDetectionList = _speedDetectionList
            .Except(_speedDetectionList.Where(t => t.Timestamp > DateTime.Now.AddSeconds(10)))
            .ToList();
    }

    #endregion speed

    #region rfid

    private void InsertToMatchingList_Rfid(IList<RfidTagDetectionResult> rfidDetections)
    {
        foreach (var detection in rfidDetections)
        {
            InsertRfidDetectionToWatchingList(detection);
            //TryToStartTimeoutTask(null, null, detection);
        }
        var logFolder = Environment.GetEnvironmentVariable(ENV_KEY.LOG_OUTPUT_PATH);

        var rfidDetectionCsvLogDirectory = Path.Join(
            logFolder,
            $"rfid_detection_{DateTime.Now:yyyyMMdd}.csv"
        );
        string[] columns = new string[] { "Timestamp", "Tag ID", "Antenna", "RSSI" };
        string[] values = new string[]
        {
            rfidDetections.First().Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS),
            rfidDetections.First().TagId,
            rfidDetections.First().Antenna.ToString(),
            rfidDetections.First().Rssi.ToString(),
        };
        WriteCsvLog(rfidDetectionCsvLogDirectory, columns, values);
    }

    private void SendToMlffWebApi_Rfid(IList<RfidTagDetectionResult> rfidDetections)
    {
        SendToMlffWebApi_RfidAsync(rfidDetections).ConfigureAwait(false);
    }

    private async Task SendToMlffWebApi_RfidAsync(IList<RfidTagDetectionResult> rfidDetections)
    {
        //string tagId = rfidDetections.First().TagId;
        //_logger.LogCritical($"current {tagId}. prev tag id {prevRfid}");
        //if (prevRfid == tagId)
        //{
        //    _logger.LogCritical("Duplicate detected. Next step is to exit this Task by throw new Eception");
        //    //throw new Exception("Duplicate detected");
        //}
        //prevRfid = tagId;
        //_logger.LogCritical($"new prev rfid{prevRfid}");

        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostRfidTagDetectionAsync(
                rfidDetections,
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                // failed to submit anpr result
                _logger.LogError(
                    "Unable to submit rfid tag detection result. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
                // TODO: insert to database as back log data
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit rfid tag detection result");
            // TODO: insert to database as back log data
        }
    }

    private void InsertRfidDetectionToWatchingList(RfidTagDetectionResult detection)
    {
        _rfidTagDetectionList.Add(detection);
        // remove staled detection from list
        _rfidTagDetectionList = _rfidTagDetectionList
            .Except(_rfidTagDetectionList.Where(t => t.Timestamp > DateTime.Now.AddSeconds(10)))
            .ToList();
    }

    #endregion rfid

    /// <summary>
    /// Method to match by using constant anpr
    /// </summary>
    /// <param name="anprDetectionResult"></param>
    private void TryToMatchTimeoutByConstant(IAnprDetectionResult? anprDetectionResult)
    {
        lock (_timeoutTasks)
        {
            var isAnpr = anprDetectionResult != null;

            // we find time from t1 to detection time,
            DateTime timestampToCheck1,
                pinpointTimestamp;
            string triggerBy;

            if (anprDetectionResult != null)
            {
                // check any timeout task is triggered by speed detection or rfid tag detection
                timestampToCheck1 = anprDetectionResult.Timestamp.AddMilliseconds(
                    -_searchWindowInMilliseconds
                );
                pinpointTimestamp = anprDetectionResult.Timestamp;
                triggerBy = TRIGGER_BY_ANPR;
            }
            else
            {
                return;
            }

            var timeoutTask = _timeoutTasks.FirstOrDefault(
                t =>
                    t.PinpointTimestamp >= timestampToCheck1
                    && t.PinpointTimestamp <= pinpointTimestamp
                    && t.TriggerBy != triggerBy
            );

            if (timeoutTask != null)
            {
                // _logger.LogDebug($"Existed timeout triggered by {timeoutTask.TriggerBy}");
                return;
            }

            timeoutTask = new TimeoutTask(pinpointTimestamp, triggerBy);
            timeoutTask.Timer.Elapsed += async (_, _) =>
            {
                await MatchDetectionResults(timeoutTask);
            };

            _timeoutTasks.Add(timeoutTask);
        }
    }

    /// <summary>
    /// Filter data based on time set
    /// </summary>
    /// <param name="timeoutTask"></param>
    /// <returns></returns>
    private async Task MatchDetectionResults(TimeoutTask timeoutTask)
    {
        var message =
            $"MATCHING DETECTION of {timeoutTask.PinpointTimestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} triggered by {timeoutTask.TriggerBy}";

        PrintWatchingList(message);

        IAnprDetectionResult? anprDetectionResult;
        ISpeedDetectionResult? speedDetectionResult;
        IRfidTagDetectionResult? rfidTagDetectionResult;
        string testPlate = "";

        lock (_anprDetectionList)
            //lock (_speedDetectionList)
                lock (_rfidTagDetectionList)
                {
                    IdentifyDetectionMatchingPairs(
                        timeoutTask,
                        out anprDetectionResult,
                        out speedDetectionResult,
                        out rfidTagDetectionResult
                    );

                    var pinpointTimestamp = timeoutTask.PinpointTimestamp.ToString(
                        TIMESTAMP_FORMAT_WITH_MILLISECONDS
                    );
                    var tagId = rfidTagDetectionResult?.TagId;
                    var speed = speedDetectionResult?.Speed;
                    var plateNumber = anprDetectionResult?.PlateNumber;
                    var cameraId = anprDetectionResult?.CameraId;
                    var antenna = rfidTagDetectionResult?.Antenna;
                    //var platePath = anprDetectionResult?.PlateImagePath;
                    //testPlate = platePath ?? testPlate;
                    var vehiclePath = anprDetectionResult?.VehicleImagePath;

                    if (!string.IsNullOrEmpty(plateNumber))
                    {
                        var prevAnpr = _anprDetectionList.FirstOrDefault(
                            t =>
                                t.Timestamp < timeoutTask.PinpointTimestamp
                                && t.Timestamp
                                    >= timeoutTask.PinpointTimestamp.AddMilliseconds(-1000)
                                && t.PlateNumber == plateNumber
                                && t.CameraId == cameraId
                        );

                        var duplicates = _anprDetectionList.FirstOrDefault(
                            t =>
                                t.Timestamp <= timeoutTask.PinpointTimestamp
                                && t.Timestamp
                                    >= timeoutTask.PinpointTimestamp.AddMilliseconds(-1000)
                                && t.PlateNumber == plateNumber
                                && t.CameraId == cameraId
                                && _anprDetectionList.Count(
                                    x =>
                                        x.Timestamp <= timeoutTask.PinpointTimestamp
                                        && x.Timestamp
                                            >= timeoutTask.PinpointTimestamp.AddMilliseconds(-1000)
                                        && x.PlateNumber == plateNumber
                                        && x.CameraId == cameraId
                                ) > 1
                        );

                        var realDuplicate = _anprDetectionList
                            .GroupBy(x => x.PlateNumber)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.ElementAt(1))
                            .FirstOrDefault();

                        if (realDuplicate != null)
                        {
                            _logger.LogCritical(realDuplicate.PlateNumber);
                            _logger.LogCritical(
                                " has been flagged due to duplication data !method realDuplicate"
                            );
                        }

                        if (duplicates != null)
                        {
                            _logger.LogCritical(
                                duplicates.PlateNumber
                                    + "has been flagged due to duplicate data and remove current one from being process for matching"
                            );
                            _logger.LogCritical("by !method duplicates");
                        }

                        if (prevAnpr != null)
                        {
                            _logger.LogCritical(
                                prevAnpr.PlateNumber
                                    + "has been flagged due to duplicate data and remove !!current one from being process for matching"
                            );
                        }
                    }

                    var matchedDetection = "Matched detection:\n";
                    matchedDetection += $"\t{"Pinpoint Timestamp", 20} >> {pinpointTimestamp}\n";
                    matchedDetection += $"\t{"RFID", 20} >> {tagId}\n";
                    matchedDetection +=
                        $"\t{"SPEED", 20} >> {(!speed.HasValue ? string.Empty : speed + "km/h")}\n";
                    matchedDetection += $"\t{"ANPR", 20} >> {plateNumber}";
                    _logger.LogInformation(matchedDetection);

                    UpdateAnalyticTable(
                        timeoutTask.PinpointTimestamp,
                        tagId ?? string.Empty,
                        speed,
                        plateNumber ?? string.Empty,
                        timeoutTask.TriggerBy,
                        cameraId ?? string.Empty,
                        antenna
                    );

                    #region obsolete
                    //alternate way to remove from cache list
                    //if (anprDetectionResult != null && _anprDetectionList.Count > 5)
                    //{
                    //    _anprDetectionList.Clear();
                    //    _logger.LogCritical(
                    //        _anprDetectionList.Count.ToString() + "anpr clear detection"
                    //    );
                    //}
                    //if (speedDetectionResult != null && _speedDetectionList.Count > 5)
                    //{
                    //    _speedDetectionList.Clear();
                    //    _logger.LogCritical(
                    //        _speedDetectionList.Count.ToString() + "speed clear detection"
                    //    );
                    //}
                    //if (rfidTagDetectionResult != null
                    ////&& _rfidTagDetectionList.Count > 5
                    //)
                    //{
                    //    _rfidTagDetectionList.Remove(rfidTagDetectionResult);
                    //    _logger.LogCritical("pinpoint ?");
                    //    //_speedDetectionList.Clear();
                    //    //_logger.LogCritical(_rfidTagDetectionList.Count.ToString());
                    //}
                    #endregion
                }
        var matchResult = new DetectionMatchResult
        {
            RfidTagDetectionResult = rfidTagDetectionResult,
            AnprDetectionResult = anprDetectionResult,
            SpeedDetectionResult = speedDetectionResult,
            DateMatched = DateTime.Now,
        };

        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostDetectionMatchAsync(
                matchResult.ToArray(),
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                // failed to submit anpr result
                _logger.LogError(
                    "Unable to submit detection match result. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
                // TODO: insert to database as back log data
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit detection match result.");
            // TODO: insert to database as back log data
        }

        PrintAnalyticTable();

        //docker logs triggered by
        _logger.LogInformation(
            $"Matching process completed for {timeoutTask.PinpointTimestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} triggered by {timeoutTask.TriggerBy}."
        );
    }

    private void IdentifyDetectionMatchingPair(
        TimeoutTask timeoutTask,
        out IAnprDetectionResult? anprDetectionResult,
        out ISpeedDetectionResult? speedDetectionResult,
        out IRfidTagDetectionResult? rfidTagDetectionResult
    )
    {
        anprDetectionResult = null;
        speedDetectionResult = null;
        rfidTagDetectionResult = null;
        DateTime? startingTimestamp,
            endingTimestamp;

        switch (timeoutTask.TriggerBy)
        {
            case TRIGGER_BY_ANPR:
                anprDetectionResult = _anprDetectionList.FirstOrDefault(
                    t => t.Timestamp == timeoutTask.PinpointTimestamp
                );

                if (anprDetectionResult == null)
                {
                    _logger.LogWarning(
                        $"Unable to get ANPR detection result of {timeoutTask.PinpointTimestamp}"
                    );
                    break;
                }

                startingTimestamp = anprDetectionResult.Timestamp;
                endingTimestamp = anprDetectionResult.Timestamp.AddMilliseconds(
                    _searchWindowInMilliseconds
                );

                speedDetectionResult = _speedDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                rfidTagDetectionResult = _rfidTagDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                break;
            case TRIGGER_BY_RFID:
                rfidTagDetectionResult = _rfidTagDetectionList.FirstOrDefault(
                    t => t.Timestamp == timeoutTask.PinpointTimestamp
                );

                if (rfidTagDetectionResult == null)
                {
                    _logger.LogWarning(
                        $"Unable to get RFID detection result of {timeoutTask.PinpointTimestamp}"
                    );
                    break;
                }

                startingTimestamp = rfidTagDetectionResult.Timestamp;
                endingTimestamp = rfidTagDetectionResult.Timestamp.AddMilliseconds(
                    _searchWindowInMilliseconds
                );

                anprDetectionResult = _anprDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                speedDetectionResult = _speedDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                break;
            case TRIGGER_BY_SPEED:
                speedDetectionResult = _speedDetectionList.FirstOrDefault(
                    t => t.Timestamp == timeoutTask.PinpointTimestamp
                );

                if (speedDetectionResult == null)
                {
                    _logger.LogWarning(
                        $"Unable to get speed detection result of {timeoutTask.PinpointTimestamp}"
                    );
                    break;
                }

                startingTimestamp = speedDetectionResult.Timestamp;
                endingTimestamp = speedDetectionResult.Timestamp.AddMilliseconds(
                    _searchWindowInMilliseconds
                );

                anprDetectionResult = _anprDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                rfidTagDetectionResult = _rfidTagDetectionList
                    .OrderBy(t => t.Timestamp)
                    .FirstOrDefault(
                        t => t.Timestamp >= startingTimestamp && t.Timestamp <= endingTimestamp
                    );

                break;
        }
    }

    /// <summary>
    /// Identifying which data is used as pair
    /// </summary>
    /// <param name="timeoutTask"></param>
    /// <param name="anprDetectionResult"></param>
    /// <param name="speedDetectionResult"></param>
    /// <param name="rfidTagDetectionResult"></param>
    private void IdentifyDetectionMatchingPairs(
        TimeoutTask timeoutTask,
        out IAnprDetectionResult? anprDetectionResult,
        out ISpeedDetectionResult? speedDetectionResult,
        out IRfidTagDetectionResult? rfidTagDetectionResult
    )
    {
        anprDetectionResult = null;
        speedDetectionResult = null;
        rfidTagDetectionResult = null;
        DateTime? pinpointTimestamp,
            beforeTimestampSpeed,
            afterTimestampSpeed,
            beforeTimestampRfid,
            afterTimestampRfid;

        try
        {
            if (timeoutTask.TriggerBy == TRIGGER_BY_ANPR)
            {
                anprDetectionResult = _anprDetectionList.FirstOrDefault(
                    t => t.Timestamp == timeoutTask.PinpointTimestamp
                );

                if (anprDetectionResult == null)
                {
                    _logger.LogWarning(
                        $"Unable to get ANPR detection result of {timeoutTask.PinpointTimestamp}"
                    );
                }
                else
                {
                    double _searchWindowSpeed = _searchWindowInMilliseconds * 2;
                    pinpointTimestamp = anprDetectionResult.Timestamp;
                    beforeTimestampSpeed = anprDetectionResult.Timestamp.AddMilliseconds(
                        -_searchWindowSpeed
                    );
                    afterTimestampSpeed = anprDetectionResult.Timestamp.AddMilliseconds(
                        _searchWindowSpeed
                    );
                    beforeTimestampRfid = anprDetectionResult.Timestamp.AddMilliseconds(
                        -_searchWindowInMilliseconds
                    );
                    afterTimestampRfid = anprDetectionResult.Timestamp.AddMilliseconds(
                        _searchWindowInMilliseconds
                    );

                    switch (anprDetectionResult.CameraId)
                    {
                        case "01":
                            if (_enableLane1)
                            {
                                if (_presenceRadarLane1)
                                {
                                    var tempSpeedDetectionResult =
                                        _speedDetectionList.FirstOrDefault(
                                            t =>
                                                t.Timestamp >= pinpointTimestamp
                                                && t.Timestamp <= afterTimestampSpeed
                                        );

                                    if (tempSpeedDetectionResult != null)
                                    {
                                        speedDetectionResult = tempSpeedDetectionResult;
                                        _logger.LogCritical(
                                            "Not found before" + afterTimestampSpeed
                                        );
                                    }
                                    else
                                    {
                                        speedDetectionResult = _speedDetectionList
                                            .Where(
                                                t =>
                                                    t.Timestamp >= beforeTimestampSpeed
                                                    && t.Timestamp <= pinpointTimestamp
                                            )
                                            .OrderByDescending(t => t.Timestamp)
                                            .FirstOrDefault();
                                        if (speedDetectionResult == null)
                                        {
                                            _logger.LogCritical(
                                                $"Not found before {beforeTimestampSpeed} and after {afterTimestampSpeed}"
                                            );
                                        }
                                    }
                                }
                                var tempRfidDetectionResultCam1 =
                                    _rfidTagDetectionList.FirstOrDefault(
                                        t =>
                                            t.Timestamp >= pinpointTimestamp
                                            && t.Timestamp <= afterTimestampRfid
                                            && (t.Antenna == 1 || t.Antenna == 2)
                                    );

                                if (tempRfidDetectionResultCam1 != null)
                                {
                                    rfidTagDetectionResult = tempRfidDetectionResultCam1;
                                    _logger.LogCritical(
                                        $"RfId found after timing {afterTimestampRfid}"
                                    );
                                }
                                else
                                {
                                    rfidTagDetectionResult = _rfidTagDetectionList
                                        .Where(
                                            t =>
                                                t.Timestamp >= beforeTimestampRfid
                                                && t.Timestamp <= pinpointTimestamp
                                                && (t.Antenna == 1 || t.Antenna == 2)
                                        )
                                        .OrderByDescending(t => t.Timestamp)
                                        .FirstOrDefault();

                                    if (rfidTagDetectionResult == null)
                                    {
                                        _logger.LogCritical(
                                            $"RfId not found before {afterTimestampRfid} and after {beforeTimestampRfid}"
                                        );
                                    }
                                    else
                                    {
                                        _logger.LogCritical(
                                            $"RfId found before timing {beforeTimestampRfid}"
                                        );
                                    }
                                }
                            }

                            break;

                        case "02":
                            if (_enableLane2)
                            {
                                if (_presenceRadarLane2)
                                {
                                    //todo in case in future there is radar on lane 2 with different ip
                                    _logger.LogCritical("Upcoming features");
                                }

                                var tempRfidDetectionResultCam2 =
                                    _rfidTagDetectionList.FirstOrDefault(
                                        t =>
                                            t.Timestamp >= pinpointTimestamp
                                            && t.Timestamp <= afterTimestampRfid
                                            && (t.Antenna == 3 || t.Antenna == 4)
                                    );

                                if (tempRfidDetectionResultCam2 != null)
                                {
                                    rfidTagDetectionResult = tempRfidDetectionResultCam2;
                                    _logger.LogCritical(
                                        $"RfId found after timing{afterTimestampRfid}"
                                    );
                                }
                                else
                                {
                                    rfidTagDetectionResult = _rfidTagDetectionList
                                        .Where(
                                            t =>
                                                t.Timestamp >= beforeTimestampRfid
                                                && t.Timestamp <= pinpointTimestamp
                                                && (t.Antenna == 3 || t.Antenna == 4)
                                        )
                                        .OrderByDescending(t => t.Timestamp)
                                        .FirstOrDefault();

                                    if (rfidTagDetectionResult == null)
                                    {
                                        _logger.LogCritical(
                                            $"RfId not found before {afterTimestampRfid} and after {beforeTimestampRfid}"
                                        );
                                    }
                                    else
                                    {
                                        _logger.LogCritical(
                                            $"RfId found before timing {beforeTimestampRfid}"
                                        );
                                    }
                                }
                            }
                            break;

                        default:
                            _logger.LogError("Camera Id is not valid");
                            break;
                    }
                }
            }
            else
            {
                _logger.LogWarning("ANPR is not triggered");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private void PrintWatchingList(string message)
    {
        message += "\n\tRFID list:\n";
        if (_rfidTagDetectionList.Count == 0)
        {
            message += $"\t\t-- Empty -- \n";
        }

        message = _rfidTagDetectionList.Aggregate(
            message,
            (current, item) =>
                current
                + $"\t\t{item.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} | {item.TagId, 10}\n"
        );

        message += "\tSPEED list:\n";
        if (_speedDetectionList.Count == 0)
        {
            message += $"\t\t-- Empty -- \n";
        }

        message = _speedDetectionList.Aggregate(
            message,
            (current, item) =>
                current
                + $"\t\t{item.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} | {item.Speed, 10}\n"
        );

        message += "\tANPR list:\n";
        if (_anprDetectionList.Count == 0)
        {
            message += $"\t\t-- Empty -- \n";
        }

        message = _anprDetectionList.Aggregate(
            message,
            (current, item) =>
                current
                + $"\t\t{item.Timestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} | {item.PlateNumber, 10}\n"
        );

        _logger.LogDebug(message);
    }

    #region analytic

    private void InsertAnalyticTable(DateTime? pinpointTimestamp, string groundTruth)
    {
        lock (_analyticTable)
        {
            _analyticTable.Add(
                (
                    pinpointTimestamp?.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS) ?? string.Empty,
                    groundTruth,
                    string.Empty
                )
            );
        }
    }

    private void UpdateAnalyticTable(
        DateTime? pinpointTimestamp,
        string tagId,
        int? speed,
        string plateNumber,
        string trigger,
        string cameraId,
        ushort? antenna
    )
    {
        lock (_analyticTable)
        {
            (string pinpointTimestamp, string groundTruth, string prediction) target;
            try
            {
                //timestamp
                var groundTruthTime =
                    pinpointTimestamp?.ToString("dd/MM/yyyy HH:mm:ss.fff") ?? string.Empty;
                //trigger flag
                var triggeredByAnalytic = trigger;

                target = _analyticTable.FirstOrDefault(
                    t => t.pinpointTimestamp.Contains(groundTruthTime)
                );

                if (target.groundTruth == null)
                {
                    var table = ComposeGroundTruthMessageCsv(
                        groundTruthTime,
                        tagId,
                        speed,
                        plateNumber,
                        triggeredByAnalytic,
                        cameraId,
                        antenna
                    );
#pragma warning disable CS8604 // Possible null reference argument.
                    WriteAnalyticCsvLog(table.Columns, table.Rows);
#pragma warning restore CS8604 // Possible null reference argument.

                    return;
                }

                var index = _analyticTable.IndexOf(target);
                var expectedGroundTruth = ComposeGroundTruthMessage(
                    tagId,
                    speed,
                    plateNumber,
                    triggeredByAnalytic
                );

                var isMatched = target.groundTruth.Contains(expectedGroundTruth);
                var prediction =
                    expectedGroundTruth + $"{(isMatched ? "--  MATCHED  --" : "-- UNMATCHED --")}";

                _analyticTable[index] = (target.pinpointTimestamp, target.groundTruth, prediction);
                WriteAnalyticLog(_analyticTable[index]);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }

    #region prev method
    private void TryToStartTimeoutTask(
        IAnprDetectionResult? anprDetectionResult,
        ISpeedDetectionResult? speedDetectionResult,
        IRfidTagDetectionResult? rfidTagDetectionResult
    )
    {
        lock (_timeoutTasks)
        {
            var isAnpr = anprDetectionResult != null;
            var isSpeed = speedDetectionResult != null;
            var isRfid = rfidTagDetectionResult != null;

            // we find time from t1 to detection time,
            DateTime timestampToCheck1,
                pinpointTimestamp;
            string triggerBy;

            if (!(isAnpr || isSpeed || isRfid) && (isAnpr && isSpeed && isRfid))
            {
                throw new Exception(
                    "Improper usage of the function. Either 1 of the inputs should be pass in only."
                );
            }

            if (anprDetectionResult != null)
            {
                // check any timeout task is triggered by speed detection or rfid tag detection
                timestampToCheck1 = anprDetectionResult.Timestamp.AddMilliseconds(
                    -_searchWindowInMilliseconds
                );
                pinpointTimestamp = anprDetectionResult.Timestamp;
                triggerBy = TRIGGER_BY_ANPR;
            }
            else if (speedDetectionResult != null)
            {
                timestampToCheck1 = speedDetectionResult.Timestamp.AddMilliseconds(
                    -_searchWindowInMilliseconds
                );
                pinpointTimestamp = speedDetectionResult.Timestamp;
                triggerBy = TRIGGER_BY_SPEED;
            }
            else if (rfidTagDetectionResult != null)
            {
                timestampToCheck1 = rfidTagDetectionResult.Timestamp.AddMilliseconds(
                    -_searchWindowInMilliseconds
                );
                pinpointTimestamp = rfidTagDetectionResult.Timestamp;
                triggerBy = TRIGGER_BY_RFID;
            }
            else
            {
                return;
            }

            var timeoutTask = _timeoutTasks.FirstOrDefault(
                t =>
                    t.PinpointTimestamp >= timestampToCheck1
                    && t.PinpointTimestamp <= pinpointTimestamp
                    && t.TriggerBy != triggerBy
            );

            if (timeoutTask != null)
            {
                // _logger.LogDebug($"Existed timeout triggered by {timeoutTask.TriggerBy}");
                return;
            }

            timeoutTask = new TimeoutTask(pinpointTimestamp, triggerBy);
            timeoutTask.Timer.Elapsed += async (_, _) =>
            {
                await MatchDetectionResult(timeoutTask);
            };

            _timeoutTasks.Add(timeoutTask);
        }
    }

    private async Task MatchDetectionResult(TimeoutTask timeoutTask)
    {
        var message =
            $"Matching detection of {timeoutTask.PinpointTimestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} triggered by {timeoutTask.TriggerBy}.";

        PrintWatchingList(message);

        IAnprDetectionResult? anprDetectionResult;
        ISpeedDetectionResult? speedDetectionResult;
        IRfidTagDetectionResult? rfidTagDetectionResult;

        lock (_anprDetectionList)
            lock (_speedDetectionList)
                lock (_rfidTagDetectionList)
                {
                    IdentifyDetectionMatchingPair(
                        timeoutTask,
                        out anprDetectionResult,
                        out speedDetectionResult,
                        out rfidTagDetectionResult
                    );

                    var pinpointTimestamp = timeoutTask.PinpointTimestamp.ToString(
                        TIMESTAMP_FORMAT_WITH_MILLISECONDS
                    );
                    var tagId = rfidTagDetectionResult?.TagId;
                    var speed = speedDetectionResult?.Speed;
                    var plateNumber = anprDetectionResult?.PlateNumber;
                    var cameraId = anprDetectionResult?.CameraId;
                    var antenna = rfidTagDetectionResult?.Antenna;

                    var matchedDetection = "Matched detection:\n";
                    matchedDetection += $"\t{"Pinpoint Timestamp", 20} >> {pinpointTimestamp}\n";
                    matchedDetection += $"\t{"RFID", 20} >> {tagId}\n";
                    matchedDetection +=
                        $"\t{"SPEED", 20} >> {(!speed.HasValue ? string.Empty : speed + "km/h")}\n";
                    matchedDetection += $"\t{"ANPR", 20} >> {plateNumber}";
                    _logger.LogInformation(matchedDetection);

                    UpdateAnalyticTable(
                        timeoutTask.PinpointTimestamp,
                        tagId ?? string.Empty,
                        speed,
                        plateNumber ?? string.Empty,
                        timeoutTask.TriggerBy,
                        cameraId ?? string.Empty,
                        antenna
                    );

                    //remove from cache list
                    if (anprDetectionResult != null)
                        _anprDetectionList.Remove(anprDetectionResult);
                    if (speedDetectionResult != null)
                        _speedDetectionList.Remove(speedDetectionResult);
                    if (rfidTagDetectionResult != null)
                        _rfidTagDetectionList.Remove(rfidTagDetectionResult);
                }

        var matchResult = new DetectionMatchResult
        {
            RfidTagDetectionResult = rfidTagDetectionResult,
            AnprDetectionResult = anprDetectionResult,
            SpeedDetectionResult = speedDetectionResult,
            DateMatched = DateTime.Now,
        };

        try
        {
            var submitResultResponseBody = await _mlffWebApiManager.PostDetectionMatchAsync(
                matchResult.ToArray(),
                _cancellationToken.Token
            );

            if (submitResultResponseBody.StatusCode != StatusCodes.Status200OK)
            {
                // failed to submit anpr result
                _logger.LogError(
                    "Unable to submit detection match result. Response payload: {ResponseMessage}",
                    submitResultResponseBody.Serialize()
                );
                // TODO: insert to database as back log data
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to submit detection match result.");
            // TODO: insert to database as back log data
        }

        PrintAnalyticTable();

        //docker logs triggered by
        _logger.LogInformation(
            $"Matching process completed for {timeoutTask.PinpointTimestamp.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)} triggered by {timeoutTask.TriggerBy}."
        );
    }
    #endregion

    private string ComposeGroundTruthMessage(
        string tagId,
        int? speed,
        string plateNumber,
        string trigger
    )
    {
        var haveRfid = !string.IsNullOrEmpty(tagId);
        var haveSpeed = speed.HasValue;
        var haveAnpr = !string.IsNullOrEmpty(plateNumber);
        var haveTrigger = !string.IsNullOrEmpty(trigger);

        return $"{(haveRfid ? tagId : "---"), 30} | "
            + $"{(haveSpeed ? speed + "km/h" : "---"), 10} | "
            + $"{(haveAnpr ? plateNumber : "---"), 10} | "
            + $"{(haveTrigger ? trigger : "---"), 10}";
    }

    private StructureFormatCSV ComposeGroundTruthMessageCsv(
        string groundTruthTime,
        string tagId,
        int? speed,
        string plateNumber,
        string trigger,
        string cameraId,
        ushort? antenna
    )
    {
        StructureFormatCSV data = new();
        var haveTimestamp = !string.IsNullOrEmpty(groundTruthTime);
        var haveTrigger = !string.IsNullOrEmpty(trigger);
        var haveRfid = !string.IsNullOrEmpty(tagId);
        var haveSpeed = speed.HasValue;
        var haveAnpr = !string.IsNullOrEmpty(plateNumber);

        data.Columns = new string[]
        {
            "Timestamp",
            "Plate Number",
            "Speed",
            "RFID",
            "Triggered By"
        };
        data.Rows = new string[]
        {
            haveTimestamp ? groundTruthTime : "---",
            haveAnpr ? plateNumber + $" ({cameraId})" : "---",
            haveSpeed ? speed.ToString() + "km/h" : "---",
            haveRfid ? tagId + $" ({antenna})" : "---",
            haveTrigger ? trigger : "---"
        };

        return data;
    }

    private void PrintAnalyticTable()
    {
        _analyticTable = _analyticTable.TakeLast(10).ToList();
        // print analytic table
        var message =
            $"\t{"Pinpoint Timestamp", 25} | "
            + $"{"TagId", 30} | {"Speed", 10} | {"Plate", 10} | {"Sequence", 20} || "
            + $"{"TagId", 30} | {"Speed", 10} | {"Plate", 10} | Correctness\n"
            + $"\t{string.Concat(Enumerable.Repeat("=", 190))}\n";

        message = _analyticTable.Aggregate(
            message,
            (current, match) => current + $"\t{ComposeAnalyticRecord(match)}\n"
        );
    }

    private string ComposeAnalyticRecord(
        (string pinpointTimestamp, string groundTruth, string prediction) match
    )
    {
        return $"{match.pinpointTimestamp, 25} | {match.groundTruth, 79} || {match.prediction}";
    }

    private void WriteAnalyticLog(
        (string pinpointTimestamp, string groundTruth, string prediction) match
    )
    {
        var logFolder = Environment.GetEnvironmentVariable(ENV_KEY.LOG_OUTPUT_PATH);

        if (string.IsNullOrEmpty(logFolder) && OperatingSystem.IsWindows())
        {
            logFolder = @"C:/Recogine/MLFF";
        }
        else if (string.IsNullOrEmpty(logFolder) && OperatingSystem.IsLinux())
        {
            logFolder = @"~/MLFF";
        }
        else if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            throw new NotSupportedException("Match analytic log only supported Windows or Linux");
        }

#pragma warning disable CS8604 // Possible null reference argument.
        var analyticLogDirectory = Path.Combine(logFolder, "match_analytic.log");
#pragma warning restore CS8604 // Possible null reference argument.
        if (!string.IsNullOrEmpty(analyticLogDirectory))
        {
            Directory.CreateDirectory(logFolder);

            WriteTextLog(analyticLogDirectory, ComposeAnalyticRecord(match) + "\n");
        }
    }

    private void WriteAnalyticCsvLog(string[] header, string[] data)
    {
        var logFolder = Environment.GetEnvironmentVariable(ENV_KEY.LOG_OUTPUT_PATH);

        if (string.IsNullOrEmpty(logFolder) && OperatingSystem.IsWindows())
        {
            logFolder = @"C:/Recogine/MLFF";
        }
        else if (string.IsNullOrEmpty(logFolder) && OperatingSystem.IsLinux())
        {
            logFolder = @"~/MLFF";
        }
        else if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            throw new NotSupportedException("Match analytic log only supported Windows or Linux");
        }

#pragma warning disable CS8604 // Possible null reference argument.
        var analyticLogCsvDirectory = Path.Combine(logFolder, "match_analytic.csv");
#pragma warning restore CS8604 // Possible null reference argument.

        if (!string.IsNullOrEmpty(analyticLogCsvDirectory))
        {
            string[] columns = header;
            string[] values = data;
            WriteCsvLog(analyticLogCsvDirectory, columns, values);
        }
    }

    #endregion

    #region simulation

    private List<(string pinpointTimestamp, string groundTruth, string prediction)> _analyticTable =
        new();

    private void StartSimulate()
    {
        if (!_enableSimulation)
        {
            throw new ApplicationException("This should be called only for simulation.");
        }

        var VEHICLE_SIMULATION_COUNT = int.Parse(
            Environment.GetEnvironmentVariable("SIMULATION_COUNT") ?? "0"
        );
        Task.Run(async () =>
        {
            var random = new Random();
            var index = 1;

            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(
                    random.Next(
                        SIMULATION_DELAY_MIN_MILLISECONDS,
                        SIMULATION_DELAY_MAX_MILLISECONDS
                    )
                );
                var plateNumber = $"ABC{index:0000}";
                var speed = index;
                var tagId = $"{index:0000 0000 0000 0000 0000 0000}";
                ushort antenna = (ushort)2;
                double rssi = 23.43;

                var haveRfid = random.Next(100) <= 60; // Assume possibility to have rfid is 60%
                var haveSpeed = random.Next(100) <= 50; // Assume possibility to have speed is 50%
                var haveAnpr = random.Next(100) <= 90; // Assume possibility to have anpr is 90%

                if (!haveRfid && !haveSpeed && !haveAnpr)
                {
                    continue;
                }

                DateTime? pinpointTimestamp = null;

                // shuffle the sequence of detections
                var detections = new[]
                {
                    haveRfid ? TRIGGER_BY_RFID : string.Empty,
                    haveAnpr ? TRIGGER_BY_ANPR : string.Empty,
                    haveSpeed ? TRIGGER_BY_SPEED : string.Empty
                };

                detections.Shuffle();

                foreach (var detection in detections)
                {
                    switch (detection)
                    {
                        case TRIGGER_BY_RFID:
                            SimulateRfidDetection(tagId, ref pinpointTimestamp, antenna, rssi);
                            break;
                        case TRIGGER_BY_ANPR:
                            SimulateAnprDetection(plateNumber, ref pinpointTimestamp);
                            break;
                        case TRIGGER_BY_SPEED:
                            SimulateSpeedDetection(speed, ref pinpointTimestamp);
                            break;
                    }
                }

                var sequence = string.Join(", ", detections.Where(t => !string.IsNullOrEmpty(t)));
                PrintSimulationConditionMessage(
                    index,
                    pinpointTimestamp,
                    haveRfid,
                    tagId,
                    haveSpeed,
                    speed,
                    haveAnpr,
                    plateNumber,
                    sequence
                );

                var groundTruth =
                    ComposeGroundTruthMessage(
                        haveRfid ? tagId : string.Empty,
                        haveSpeed ? speed : null,
                        haveAnpr ? plateNumber : string.Empty,
                        string.Empty
                    ) + $"{sequence, 20}";

                InsertAnalyticTable(pinpointTimestamp, groundTruth);

                if (index >= VEHICLE_SIMULATION_COUNT)
                {
                    break;
                }

                index++;
            }

            _logger.LogDebug("Simulation completed");
        });
    }

    private void PrintSimulationConditionMessage(
        int index,
        DateTime? pinpointTimestamp,
        bool haveRfid,
        string tagId,
        bool haveSpeed,
        int speed,
        bool haveAnpr,
        string plateNumber,
        string sequence
    )
    {
        var conditionMessage = $"Condition {index}:\n";
        conditionMessage +=
            $"\tPinpoint Timestamp: {pinpointTimestamp?.ToString(TIMESTAMP_FORMAT_WITH_MILLISECONDS)}\n";
        conditionMessage +=
            $"\t{(haveRfid ? "(x)" : "( )")} RFID {(haveRfid ? $"- {tagId}" : string.Empty)}\n";
        conditionMessage +=
            $"\t{(haveSpeed ? "(x)" : "( )")} SPEED {(haveSpeed ? $"- {speed}km/h" : string.Empty)}\n";
        conditionMessage +=
            $"\t{(haveAnpr ? "(x)" : "( )")} ANPR {(haveAnpr ? $"- {plateNumber}" : string.Empty)}\n";
        conditionMessage += $"\tSequence: {sequence:15}\n";
        _logger.LogDebug(conditionMessage);
    }

    private void SimulateAnprDetection(string plateNumber, ref DateTime? pinpointTimestamp)
    {
        var detection = Simulator.CreateAnprDetectionTestData(plateNumber);
        InsertToMatchingList_Anpr(detection);
        // SendToMlffWebApi_Anpr(detection);
        pinpointTimestamp = pinpointTimestamp ?? detection.Timestamp;
    }

    private void SimulateSpeedDetection(int speed, ref DateTime? pinpointTimestamp)
    {
        var detection = Simulator.CreateSpeedDetectionTestData(speed);
        InsertToMatchingList_Speed(detection);
        // SendToMlffWebApi_Speed(detection);
        pinpointTimestamp = pinpointTimestamp ?? detection.Timestamp;
    }

    private void SimulateRfidDetection(
        string tagId,
        ref DateTime? pinpointTimestamp,
        ushort antenna,
        double rssi
    )
    {
        var detection = Simulator.CreateRfidTagDetectionTestData(tagId, antenna, rssi);
        InsertToMatchingList_Rfid(detection);
        // SendToMlffWebApi_Rfid(detection);
        pinpointTimestamp = detection.FirstOrDefault()?.Timestamp;
    }

    #endregion

    private void WriteTextLog(string fileDirectory, string message)
    {
        if (!message.EndsWith("\n"))
        {
            message += "\n";
        }

        using (StreamWriter writer = new StreamWriter(fileDirectory, append: true, Encoding.UTF8))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            writer.WriteLine(Encoding.UTF8.GetString(bytes));
        }
    }
}

public class TimeoutTask
{
    public DateTime PinpointTimestamp { get; }

    public string TriggerBy { get; }

    public Timer Timer { get; }

    public TimeoutTask(DateTime startTimestamp, string triggerBy)
    {
        PinpointTimestamp = startTimestamp;
        TriggerBy = triggerBy;

        Timer = new Timer();
        Timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
        Timer.AutoReset = false;
        Timer.Start();
    }
}

public class StructureFormatCSV
{
    public string[]? Columns { get; set; }
    public string[]? Rows { get; set; }
}

public static class ArrayExtensions
{
    public static void Shuffle<T>(this T[] array)
    {
        var random = new Random();
        var n = array.Length;
        while (n > 1)
        {
            var k = random.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}
