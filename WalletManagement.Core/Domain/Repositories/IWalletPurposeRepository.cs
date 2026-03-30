using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IWalletPurposeRepository : IGenericRepository<WalletPurpose>
    {
        //public Task<bool> IsPurposeExistsWithNameAsync(string name);

        public Task<IEnumerable<WalletPurpose>> ListAllPurposeAsync();

        public Task<WalletPurpose> GetPurposeById(int id);

        public Task<WalletPurpose> GetPurposeByNameAsync(string name);
    }
}
