using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface IConfigurationService
    {
        Task<IList<string>> GetAllScopes();
        Task<IList<string>> GetAllGrantTypes();

        // FIX: Add the 'where T : class' constraint here
        T GetPlainConfiguration<T>(string configName) where T : class;

        // FIX: Add the 'where T : class' constraint here
        T GetConfiguration<T>(string configName) where T : class;

        // FIX: Add the 'where T : class' constraint here
        Task<T> GetConfigurationAsync<T>(string configName) where T : class;

        Task<string> GetActiveAuthenticationId();
        Task<ConfigurationResponse> UpdateDefaultAuthScheme(string Id);
    }
}