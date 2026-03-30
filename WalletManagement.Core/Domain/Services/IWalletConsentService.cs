using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Domain.Services
{
    public interface IWalletConsentService
    {
        public Task<ServiceResult> GetAllConsentAsync();

        public Task<ServiceResult> GetActiveConsentAsync();

        public Task<ServiceResult> GetConsentsByUserIdAsync(string Id);

        public Task<ServiceResult> GetActiveConsentsByUserIdAsync(string Id);
        public Task<ServiceResult> AddConsent(WalletConsentDTO walletConsentDTO);
        public Task<ServiceResult> RevokeConsent(int id);
    }
}
