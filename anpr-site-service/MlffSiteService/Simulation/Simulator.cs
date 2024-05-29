using MlffSiteService.Interface;
using MlffSiteService.Models;

namespace MlffSiteService.Simulation;

public static class Simulator
{
    public static AnprDetectionResultRecoAnpr CreateAnprDetectionTestData(string plateNumber)
    {
        // var plateNumber = CreatePlateNumber();
        var detectionResult = new AnprDetectionResultRecoAnpr
        {
            CameraId = "01",
            ImageId = "",
            Location = "",
            ProcessTime = 100,
            Left = 0,
            Right = 0,
            Top = 0,
            Bottom = 0,
            Confidence = 95,
            Timestamp = DateTime.Now,
            PlateImagePath = $"/path-to-plate-image_{plateNumber.ToLower()}.jpg",
            PlateNumber = plateNumber,
            VehicleImagePath = $"/path-to-vehicle-image_{plateNumber.ToLower()}.jpg"
        };

        return detectionResult;
    }

    private static string CreatePlateNumber()
    {
        var random = new Random();
        var plateNumber = string.Join("", Enumerable.Range(0, 3).Select(t => (char)random.Next(65, 90))) + " " + random.Next(0, 9999).ToString();
        return plateNumber;
    }

    public static ISpeedDetectionResult CreateSpeedDetectionTestData(int speed)
    {
        // return new SpeedDetectionResult(DateTime.Now, new Random().Next(40, 100));
        return new SpeedDetectionResult(DateTime.Now, speed);
    }

    public static IList<RfidTagDetectionResult> CreateRfidTagDetectionTestData(string tagId, ushort antenna, double rssi)
    {
        var list = new List<RfidTagDetectionResult>();

        // var tagId = CreateTagId();
        list.Add(new RfidTagDetectionResult(DateTime.Now, tagId, antenna, rssi));

        return list;
    }

    private static string CreateTagId()
    {
        var random = new Random();
        var tagId = string.Join(" ", Enumerable.Range(0, 6).Select(t => random.Next(0, 65535).ToString("X4")));
        return tagId;
    }

}