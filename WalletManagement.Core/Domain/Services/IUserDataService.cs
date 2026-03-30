using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface IUserDataService
    {
        public Task<ServiceResult> GetProfile(string url);
    }
}
