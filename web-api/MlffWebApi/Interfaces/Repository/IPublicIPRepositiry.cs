using MlffWebApi.DTO.IP;
using MlffWebApi.Interfaces.PublicIP;

namespace MlffWebApi.Interfaces.Repository
{
    public interface IPublicIPRepository
    {
        /// <summary>
        /// Get Public IP address
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IEnumerable entity</returns>
        Task<IPublicIPRecognition> GetPublicIP(string siteId, CancellationToken cancellationToken);

        ///<summary>
        /// Add public IP if not exist otherwise updated
        /// </summary>
        /// <param name="records">Public IP detection result</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<(IList<IPublicIPRecognition> updatedRecords, string details)> AddRecordsIfNotExist(
            ICollection<IPublicIPRecognition> records , CancellationToken cancellationToken);
    }
}
