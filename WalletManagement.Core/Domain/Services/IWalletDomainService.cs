using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface IWalletDomainService
    {
        //public Task<WalletDomainResponse> CreateDomainAsync(WalletDomain walletDomain);
        public Task<WalletDomain> GetWalletDomainAsync(int id);
        public Task<WalletDomainResponse> UpdateWalletDomainAsync(WalletDomain walletDomain);

        public Task<IEnumerable<WalletDomain>> ListWalletDomainAsync();

        public Task<WalletDomainResponse> DeleteWalletDomainAsync(int id, string updatedBy);

        public Task<ServiceResult> GetWalletDomainsList();
    }
}
