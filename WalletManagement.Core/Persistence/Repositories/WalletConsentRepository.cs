using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class WalletConsentRepository : GenericRepository<WalletConsent, idp_dtplatformContext>,
        IWalletConsentRepository
    {
        private readonly ILogger _logger;

        public WalletConsentRepository(idp_dtplatformContext context, ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<WalletConsent>> GetAllConsentsAsync()
        {
            try
            {
                return await Context.WalletConsents.ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetAllConsentsAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<WalletConsent>> GetActiveConsentsAsync()
        {
            try
            {
                return await Context.WalletConsents.
                    Where(u => u.Status == "ACTIVE").ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveConsentsAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<WalletConsent>> GetActiveConsentsByUserIdAsync(string Id)
        {
            try
            {
                return await Context.WalletConsents.
                    Where(u => u.Status == "ACTIVE" && u.Suid == Id).ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveConsentsByUserIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<WalletConsent>> GetConsentsByUserIdAsync(string Id)
        {
            try
            {
                return await Context.WalletConsents.
                    Where(u => u.Suid == Id).ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetConsentsByUserIdAsync::Database exception");
                return null;
            }
        }

        public async Task<WalletConsent> GetConsentByIdAsync(int Id)
        {
            try
            {
                return await Context.WalletConsents.
                    SingleOrDefaultAsync(u => u.Id == Id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetConsentByIdAsync::Database exception");
                return null;
            }
        }
    }
}