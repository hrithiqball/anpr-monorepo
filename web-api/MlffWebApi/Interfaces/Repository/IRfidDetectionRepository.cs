using MlffWebApi.DTO.RfidDetection;
using MlffWebApi.Interfaces.RFID;

namespace MlffWebApi.Interfaces.Repository;

public interface IRfidDetectionRepository
{
    /// <summary>
    /// Get RFID tag detection records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IEnumerable entity</returns>
    Task<IPaginationResult<IEnumerable<IRfidTagDetection>>> GetRecords(
        GetRfidTagDetectionListRequestDto requestDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Add RFID tag detection record if not exist, otherwise update.
    /// </summary>
    /// <param name="record">LPR detection result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IRfidTagDetection> AddRecordIfNotExist(IRfidTagDetection record,
        CancellationToken cancellationToken);

    /// <summary>
    /// Add RFID tag detection records if not exist, otherwise update.
    /// </summary>
    /// <param name="records">Collection of LPR detection result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<(IList<IRfidTagDetection> updatedRecords, string details)> AddRecordsIfNotExist(
        ICollection<IRfidTagDetection> records, CancellationToken cancellationToken);

}