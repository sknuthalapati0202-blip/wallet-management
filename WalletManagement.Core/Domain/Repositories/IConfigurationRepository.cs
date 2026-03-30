using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IConfigurationRepository : IGenericRepository<Configuration>
    {
        public Configuration GetConfigurationByName(string name);
        public Task<Configuration> GetConfigurationByNameAsync(string name);
    }
}
