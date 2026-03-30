using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IWalletConfigurationRepository : IGenericRepository<WalletConfiguration>
    {
        public Task<IEnumerable<WalletConfiguration>> GetWalletConfigurationList();
        public Task<string> GetCredentialFormats();
    }
}
