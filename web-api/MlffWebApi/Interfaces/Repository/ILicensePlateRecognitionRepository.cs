using MlffWebApi.DTO.LicensePlateRecognition;
using MlffWebApi.Interfaces.ANPR;

namespace MlffWebApi.Interfaces.Repository;

public interface ILicensePlateRecognitionRepository
{
    /// <summary>
    /// Get LPR records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IEnumerable entity</returns>
    Task<IPaginationResult<IEnumerable<ILicensePlateRecognition>>> GetRecords(
        GetLicensePlateRecognitionListRequestDto requestDto,
        CancellationToken cancellationToken);

    /// <summary>
    /// Add LPR record if not exist, otherwise update.
    /// </summary>
    /// <param name="record">LPR detection result</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ILicensePlateRecognition> AddRecordIfNotExist(ILicensePlateRecognition record,
        CancellationToken cancellationToken);

    /// <summary>
    /// Insert LPR records if not exist, otherwise update.
    /// </summary>
    /// <param name="records">Collection of LPR detection result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<(IList<ILicensePlateRecognition> updatedRecords, string details)> AddRecordsIfNotExist(
        ICollection<ILicensePlateRecognition> records, CancellationToken cancellationToken);

    /// <summary>
    /// Delete record
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<bool> DeleteRecord(Guid uid, CancellationToken cancellationToken);

    /// <summary>
    /// Delete multiple records between <paramref name="start"/> to <paramref name="end"/>
    /// </summary>
    /// <param name="site">Site</param>
    /// <param name="start">Starting timestamp</param>
    /// <param name="end">Ending timestamp</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of records deleted</returns>
    Task<int> DeleteRecords(ISite site, DateTime start, DateTime end, CancellationToken cancellationToken);
}