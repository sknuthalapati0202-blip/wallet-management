using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    internal class ProvisionStatusRepository : GenericRepository<ProvisionStatus, idp_dtplatformContext>,
            IProvisionStatusRepository
    {
        private readonly ILogger _logger;
        public ProvisionStatusRepository(idp_dtplatformContext context, ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }
        public async Task<ProvisionStatus> GetProvisionStatus(string suid, string credentialId)
        {
            try
            {
                return await Context.ProvisionStatuses
                    .AsNoTracking()
                    .Where(u => u.Suid == suid && u.CredentialId == credentialId)
                    .OrderByDescending(u => u.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetProvisionStatus::Database exception");
                return null;
            }
        }
        public async Task<ProvisionStatus> GetProvisionStatusByDocumentId(string credentialId, string documentId)
        {
            try
            {
                return await Context.ProvisionStatuses
                    .AsNoTracking()
                    .Where(u => u.DocumentId == documentId && u.CredentialId == credentialId)
                    .OrderByDescending(u => u.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetProvisionStatus::Database exception");
                return null;
            }
        }
    }
}
