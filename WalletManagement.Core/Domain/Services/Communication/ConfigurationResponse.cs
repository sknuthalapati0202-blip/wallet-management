using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Services.Communication
{
    public class ConfigurationResponse : BaseResponse<Configuration>
    {
        public ConfigurationResponse(Configuration category) : base(category) { }

        public ConfigurationResponse(string message) : base(message) { }

        public ConfigurationResponse(Configuration category, string message) :
            base(category, message)
        { }
    }

    public class configurationMCRequest
    {
        public string configName { get; set; }
        public object requestData { get; set; }
    }
}
