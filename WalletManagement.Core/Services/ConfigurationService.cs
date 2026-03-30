using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.Utilities;

namespace WalletManagement.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private IUnitOfWork _unitOfWork;
        // Initialize logger.
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IUnitOfWork unitOfWork,
            ILogger<ConfigurationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IList<string>> GetAllScopes()
        {
            _logger.LogDebug("-->GetAllScopes");

            // Get Configuration
            var configObject = await GetConfigurationAsync<JObject>
                ("IDP_Configuration");
            if (null == configObject)
            {
                _logger.LogError("GetConfigurationAsync Failed");
                return null;
            }

            // Get supported scopes list
            var openidconnect = configObject.SelectToken("openidconnect");
            if (null == openidconnect)
            {
                _logger.LogError("Get scopes_supported Failed");
                return null;
            }


            // Get supported scopes list
            var scopes_supported = openidconnect.SelectToken("scopes_supported")
                .Values<string>().ToList();
            if (null == scopes_supported)
            {
                _logger.LogError("Get scopes_supported Failed");
                return null;
            }

            return scopes_supported;
        }

        public async Task<IList<string>> GetAllGrantTypes()
        {
            _logger.LogDebug("-->GetAllGrantTypes");

            // Get Configuration
            var configObject = await GetConfigurationAsync<JObject>
                ("IDP_Configuration");
            if (null == configObject)
            {
                _logger.LogError("GetConfigurationAsync Failed");
                return null;
            }

            // Get supported scopes list
            var openidconnect = configObject.SelectToken("openidconnect");
            if (null == openidconnect)
            {
                _logger.LogError("Get scopes_supported Failed");
                return null;
            }

            // Get supported grant types
            var grantTypesSupported = openidconnect.SelectToken("grant_types_supported")
                .Values<string>().ToList();
            if (null == grantTypesSupported)
            {
                _logger.LogError("Get scopes_supported Failed");
                return null;
            }

            _logger.LogDebug("<--GetAllGrantTypes");
            return grantTypesSupported;
        }

        // FIX: Added 'where T : class' constraint
        public T GetPlainConfiguration<T>(string configName) where T : class
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = _unitOfWork.Configuration.
                GetConfigurationByName(configName);
            if (null == configRecord || null == configRecord.Value)
            {
                _logger.LogError("Get Configuration Record Failed in GetPlainConfiguration");
                return default;
            }


            // Convert Plain data string to object
            T config = JsonConvert.DeserializeObject<T>(configRecord.Value);
            if (null == config) // This check is now safe
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }

        // FIX: Added 'where T : class' constraint
        public T GetConfiguration<T>(string configName) where T : class
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = _unitOfWork.Configuration.
                GetConfigurationByName(configName);
            if (null == configRecord || null == configRecord.Value)
            {
                _logger.LogError("ConfigName - " + configName.ToString());
                _logger.LogError("Get Configuration Record Failed in GetConfiguration");
                return default;
            }

            // Get Plain data from secured data
            var plainData = PKIMethods.Instance.
                PKIDecryptSecureWireData(configRecord.Value);
            if (null == plainData)
            {
                _logger.LogError("PKIDecryptSecureWireData Failed");
                return default;
            }

            // Convert Plain data string to object
            T config = JsonConvert.DeserializeObject<T>(plainData);
            if (null == config) // This check is now safe
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }

        // FIX: Added 'where T : class' constraint
        public async Task<T> GetConfigurationAsync<T>(string configName) where T : class
        {
            _logger.LogDebug("-->GetConfiguration");

            // Get Configuration Record
            var configRecord = await _unitOfWork.Configuration.
                GetConfigurationByNameAsync(configName);
            if (null == configRecord || null == configRecord.Value)
            {
                _logger.LogError("Get Configuration Record Failed in GetConfigurationAsync");
                return default;
            }

            // Get Plain data from secured data
            var plainData = PKIMethods.Instance.
                PKIDecryptSecureWireData(configRecord.Value);
            if (null == plainData)
            {
                _logger.LogError("PKIDecryptSecureWireData Failed");
                return default;
            }

            // Convert Plain data string to object
            T config = JsonConvert.DeserializeObject<T>(plainData);
            if (null == config) // This check is now safe
            {
                _logger.LogError("Convert Plain data string to object Failed");
                return default;
            }

            _logger.LogDebug("<--GetConfiguration");
            return config;
        }

        public async Task<string> GetActiveAuthenticationId()
        {
            var res = await _unitOfWork.Configuration.GetConfigurationByNameAsync("DEFAULT_AUTH_SCHEME");
            if (res == null)
            {
                return "";
            }
            return res.Value;
        }

        public async Task<ConfigurationResponse> UpdateDefaultAuthScheme(string Id)
        {
            try
            {
                var configuration = await _unitOfWork.Configuration.GetConfigurationByNameAsync("DEFAULT_AUTH_SCHEME");

                configuration.Value = Id;

                _unitOfWork.Configuration.Update(configuration);

                await _unitOfWork.SaveAsync();

                return new ConfigurationResponse(configuration, "Configuration updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateDefaultAuthScheme::Database exception");
                return new ConfigurationResponse("Failed to update Configuration");
            }
        }
    }
}