using MlffWebApi.Interfaces.ANPR;
using MlffWebApi.Interfaces.PublicIP;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.Interfaces.Hubs;

public interface IDetectionHub
{
    Task LicensePlateDetected(ILicensePlateRecognitionLite lpr);
    Task SpeedDetected(ISpeedDetectionLite speed);
    Task RfidTagDetected(IRfidTagDetectionLite tag);
    Task DetectionMatched(IDetectionMatchLite detectionMatch);
    Task PublicIPRetrieved(IPublicIPRecognition publicIP);
}