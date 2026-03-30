using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserDataService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        public UserDataService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ILogger<UserDataService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ServiceResult> GetProfile(string url)
        {
            try
            {
                HttpClient _client = new HttpClient();

                HttpResponseMessage result;

                result = await _client.GetAsync(url);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new ServiceResult(false, "Internal error");
                }

                var responseString = await result.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<APIResponse>(responseString);
                if (apiResponse == null)
                {
                    return new ServiceResult(false, "Internal error");
                }
                if (!apiResponse.Success)
                {
                    return new ServiceResult(false, apiResponse.Message);
                }
                return new ServiceResult(true, apiResponse.Message, apiResponse.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new ServiceResult(false, ex.Message);
            }
        }
    }
}
