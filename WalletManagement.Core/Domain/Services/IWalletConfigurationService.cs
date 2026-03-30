using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface IWalletConfigurationService
    {
        public Task<ServiceResult> GetWalletConfiguration();
        public Task<ServiceResult> UpdateWalletConfiguration(WalletConfigurationResponse walletConfigurationResponse);
        public Task<ServiceResult> GetConfiguration();
        public Task<ServiceResult> GetWalletConfigurationDetails();
    }
}
