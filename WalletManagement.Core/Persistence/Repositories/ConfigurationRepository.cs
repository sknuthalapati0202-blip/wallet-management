using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class ConfigurationRepository : GenericRepository<Configuration, idp_dtplatformContext>,
            IConfigurationRepository
    {
        private readonly ILogger _logger;
        public ConfigurationRepository(idp_dtplatformContext context,
            ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }

        public Configuration GetConfigurationByName(string name)
        {
            try
            {
                return Context.Configurations.FirstOrDefault(c => c.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetConfigurationByName:: Database exception");
                return null;
            }
        }

        public async Task<Configuration> GetConfigurationByNameAsync(string name)
        {
            try
            {
                return await Context.Configurations.FirstOrDefaultAsync(c => c.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetConfigurationByNameAsync:: Database exception");
                return null;
            }
        }
    }
}
