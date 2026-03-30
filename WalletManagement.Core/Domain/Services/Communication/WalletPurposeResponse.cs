using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Services.Communication
{
    public class WalletPurposeResponse : BaseResponse<WalletPurpose>
    {
        public WalletPurposeResponse(WalletPurpose category) : base(category) { }

        public WalletPurposeResponse(string message) : base(message) { }

        public WalletPurposeResponse(WalletPurpose category, string message) : base(category, message) { }
    }
}
