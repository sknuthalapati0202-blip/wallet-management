using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IWalletConsentRepository : IGenericRepository<WalletConsent>
    {
        public Task<IEnumerable<WalletConsent>> GetAllConsentsAsync();
        public Task<IEnumerable<WalletConsent>> GetActiveConsentsAsync();
        public Task<IEnumerable<WalletConsent>> GetActiveConsentsByUserIdAsync(string Id);
        public Task<IEnumerable<WalletConsent>> GetConsentsByUserIdAsync(string Id);
        public Task<WalletConsent> GetConsentByIdAsync(int Id);
    }
}
