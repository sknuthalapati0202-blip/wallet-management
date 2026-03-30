using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class WalletConfigurationRepository : GenericRepository<WalletConfiguration, idp_dtplatformContext>,
            IWalletConfigurationRepository
    {
        private readonly ILogger _logger;

        public WalletConfigurationRepository(idp_dtplatformContext context, ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<WalletConfiguration>> GetWalletConfigurationList()
        {
            try
            {
                return await Context.WalletConfigurations.AsNoTracking().ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetWalletConfigurationList::Database exception");
                return null;
            }
        }

        public async Task<string> GetCredentialFormats()
        {
            try
            {
                var credentialFormatData = await Context.WalletConfigurations.FirstOrDefaultAsync(d => d.Name == "Credentials_Formats");

                return credentialFormatData.Value;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetCredentialFormats::Database exception");
                return null;
            }
        }
    }
}