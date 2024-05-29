using MlffWebApi.DTO.DetectionMatch;

namespace MlffWebApi.Interfaces.Repository;

public interface IDetectionMatchRepository
{
    
    /// <summary>
    /// Insert match records
    /// </summary>
    /// <param name="records">Collection of LPR detection result</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<(IList<IDetectionMatch> addedRecords, string details)> AddRecords(
        ICollection<IDetectionMatch> records, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get match records
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IEnumerable entity</returns>
    Task<IPaginationResult<IEnumerable<IDetectionMatch>>> GetRecords(
        GetDetectionMatchListRequestDto requestDto,
        CancellationToken cancellationToken);
}