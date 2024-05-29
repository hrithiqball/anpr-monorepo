using Microsoft.EntityFrameworkCore;
using MlffWebApi.Database.DbContexts;
using MlffWebApi.Interfaces;
using MlffWebApi.Interfaces.PublicIP;
using MlffWebApi.Interfaces.Repository;
using MlffWebApi.Models.PublicIP;

namespace MlffWebApi.Repositories
{
    public class PublicIPRecognitionRepository : IPublicIPRepository
    {
        private readonly MlffDbContext _dbContext;
        private readonly ILogger<PublicIPRecognitionRepository> _logger;

        public PublicIPRecognitionRepository(MlffDbContext context, ILoggerFactory loggerFactory)
        {
            _dbContext = context;
            _logger = loggerFactory.CreateLogger<PublicIPRecognitionRepository>();
        }

        public async Task<IPublicIPRecognition> GetPublicIP(
            string siteId,
            CancellationToken cancellationToken
        )
        {
            try
            {
                return await _dbContext.public_ip_recognitions
                    .Where(t => t.site_id.ToLower() == siteId.ToLower())
                    .OrderByDescending(t => t.date_update)
                    .Select(t => t.ToBusinessModel())
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<(
            IList<IPublicIPRecognition> updatedRecords,
            string details
        )> AddRecordsIfNotExist(
            ICollection<IPublicIPRecognition> records,
            CancellationToken cancellationToken
        )
        {
            var currentDateTime = DateTime.Now;

            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var targets = _dbContext.public_ip_recognitions.Where(
                t => t.date_update <= currentDateTime
            );

            var resultList = new List<IPublicIPRecognition>();
            var updateRow = 0;
            var insertRow = 0;

            foreach (var record in records)
            {
                var target = targets.FirstOrDefault(
                    t => t.site_id == record.SiteId && t.ip_address == record.PublicIPString
                );

                var isInsert = target == null;

                await AddOrInsertPublicIPRecognition(cancellationToken, record, ref target);

                if (isInsert)
                {
                    insertRow++;
                }
                else
                {
                    updateRow++;
                }

                resultList.Add(target.ToBusinessModel());
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var details = $"{insertRow} rows inserted, {updateRow} rows updated.";

            return (resultList, details);
        }

        private Task AddOrInsertPublicIPRecognition(
            CancellationToken cancellationToken,
            IPublicIPRecognition record,
            ref public_ip target
        )
        {
            if (target == null)
            {
                target = record.ToDatabaseModel();
                _dbContext.public_ip_recognitions.AddAsync(target, cancellationToken);
            }
            else
            {
                target.site_id = record.SiteId;
                target.date_update = DateTime.Now;
            }

            return Task.CompletedTask;
        }
    }
}
