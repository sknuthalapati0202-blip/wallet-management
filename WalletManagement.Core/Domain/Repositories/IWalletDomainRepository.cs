using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IWalletDomainRepository : IGenericRepository<WalletDomain>
    {
        //public Task<bool> IsScopeExistsWithNameAsync(string name);

        public Task<IEnumerable<WalletDomain>> ListAllScopeAsync();

        public Task<WalletDomain> GetWalletDomainByIdWithPurposes(int id);
    }
}
