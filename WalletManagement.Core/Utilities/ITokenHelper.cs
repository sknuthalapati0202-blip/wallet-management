using Newtonsoft.Json.Linq;
using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Utilities
{
    public interface ITokenHelper
    {
        public Task<ServiceResult> IntrospectToken(string token);
        public Task<JObject?> GetUserInfo(string token);
        public Task<ServiceResult> GetUserEmail(string suid);
    }
}
