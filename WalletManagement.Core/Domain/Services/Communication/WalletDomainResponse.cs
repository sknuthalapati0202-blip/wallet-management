using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Services.Communication
{
    public class WalletDomainResponse : BaseResponse<WalletDomain>
    {
        public WalletDomainResponse(WalletDomain category) : base(category) { }

        public WalletDomainResponse(string message) : base(message) { }

        public WalletDomainResponse(WalletDomain category, string message) : base(category, message) { }
    }
}
