using MlffWebApi.DTO.SpeedDetection;
using MlffWebApi.Interfaces.RFID;
using MlffWebApi.Interfaces.Speed;

namespace MlffWebApi.Interfaces.Repository;

public interface ISpeedDetectionRepository
{
    /// <summary>
    /// Get speed detection records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IEnumerable entity</returns>
    Task<IPaginationResult<IEnumerable<ISpeedDetection>>> GetRecords(
        GetSpeedDetectionListRequestDto requestDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Add speed detection record if not exist, otherwise update.
    /// </summary>
    /// <param name="record">Speed detection result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ISpeedDetection> AddRecord(ISpeedDetection record, CancellationToken cancellationToken);

    /// <summary>
    /// Add speed detection records if not exist, otherwise update.
    /// </summary>
    /// <param name="records">Collection of speed detection result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<(IList<ISpeedDetection> updatedRecords, string details)> AddRecords(ICollection<ISpeedDetection> records,
        CancellationToken cancellationToken);
}