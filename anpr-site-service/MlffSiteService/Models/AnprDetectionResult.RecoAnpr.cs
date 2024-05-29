using MlffSiteService.Interface;
using Newtonsoft.Json;

namespace MlffSiteService.Models;

public class AnprDetectionResultRecoAnpr : IAnprDetectionResult
{
    /// <summary>
    ///     Camera ID
    /// </summary>
    [JsonProperty("cameraId")]
    public string CameraId { get; set; } = string.Empty;

    /// <summary>
    ///     Image ID
    /// </summary>
    [JsonProperty("imageID")]
    public string ImageId { get; set; } = string.Empty;

    /// <summary>
    ///     Location
    /// </summary>
    [JsonProperty("location")]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    ///     ANPR process time in milliseconds
    /// </summary>
    [JsonProperty("ProcessTime")]
    public int ProcessTime { get; set; }

    /// <summary>
    ///     License plate coordinate left
    /// </summary>
    [JsonProperty("xmin")]
    public int Left { get; set; }

    /// <summary>
    ///     License plate coordinate right
    /// </summary>
    [JsonProperty("xmax")]
    public int Right { get; set; }

    /// <summary>
    ///     License plate coordinate
    /// </summary>
    [JsonProperty("ymin")]
    public int Top { get; set; }

    /// <summary>
    /// </summary>
    [JsonProperty("ymax")]
    public int Bottom { get; set; }

    /// <summary>
    ///     The confidence level to the detection result. Range from 0 to 100.
    /// </summary>
    [JsonProperty("confidenceLevel")]
    public int Confidence { get; set; }

    /// <summary>
    ///     Detection timestamp
    /// </summary>
    [JsonProperty("createdAt")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Relative image path of number plate
    /// </summary>
    [JsonProperty("plateImagePath")]
    public string PlateImagePath { get; set; } = string.Empty;

    /// <summary>
    ///     Plate number
    /// </summary>
    [JsonProperty("plateNum")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    ///     Relative image path of vehicle
    /// </summary>
    [JsonProperty("vehicleImagePath")]
    public string VehicleImagePath { get; set; } = string.Empty;


    public IBoundingBox BoundingBox => new BoundingBox
    {
        Top = Top,
        Left = Left,
        Width = Right - Left,
        Height = Bottom - Top
    };
}