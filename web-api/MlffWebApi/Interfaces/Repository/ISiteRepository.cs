using MlffWebApi.DTO.Site;

namespace MlffWebApi.Interfaces.Repository;

public interface ISiteRepository
{
    Task<ISite> GetSite(string id, CancellationToken cancellationToken);
    Task<IList<ISite>> GetSites(CancellationToken cancellationToken);

    Task<IPaginationResult<IList<ISite>>> GetSites(GetSiteListRequestDto requestDto,
        CancellationToken cancellationToken);

    Task<IEnumerable<ISite>> AddSite(IEnumerable<AddSiteRequestDto.Site> sites, string currentUsername,
        CancellationToken cancellationToken);

    Task<ISite> AddSite(AddSiteRequestDto.Site site, string currentUsername, CancellationToken cancellationToken);

    Task<ISite> UpdateSite(string id, UpdateSiteRequestDto requestDto, string currentUser,
        CancellationToken cancellationToken);

    Task<bool> DeleteSite(string id, CancellationToken cancellationToken);
}