using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Utilities
{
    public interface IGlobalConfiguration
    {
        public SSOConfig GetSSOConfiguration();
        public string GetPKIConfiguration();
        public string GetFCMConfiguration();
        public idp_configuration GetIDPConfiguration();
        public ErrorConfiguration GetErrorConfiguration();
        public WebConstants GetWebConstantsConfiguration();
        public ThresholdConfiguration GetThresholdConfiguration();
    }
}