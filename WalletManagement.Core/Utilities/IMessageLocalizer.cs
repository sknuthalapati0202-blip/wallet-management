using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Utilities
{
    public interface IMessageLocalizer

    {
        string GetMessage(LocalizedMessage message);

    }
}
