using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly HttpClient _client;
        private readonly ILogger<OrganizationService> _logger;
        private readonly IConfiguration _configuration;

        public OrganizationService(HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OrganizationService> logger)
        {
            _configuration = configuration;
            httpClient.BaseAddress = new Uri(configuration["APIServiceLocations:OrganizationOnboardingServiceBaseAddress"]);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _client = httpClient;
            _client.Timeout = TimeSpan.FromMinutes(10);
            _logger = logger;
        }

        public async Task<ServiceResult> GetOrganizationDetailsByUIdAsync(string organizationUid)
        {
            try
            {
                _logger.LogInformation("Get organization details by id api call start");
                HttpResponseMessage response = await _client.GetAsync($"api/get/organization/detailsById/{organizationUid}");
                _logger.LogInformation("Get organization details by id api call end");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    APIResponse apiResponse = JsonConvert.DeserializeObject<APIResponse>(await response.Content.ReadAsStringAsync());
                    if (apiResponse.Success)
                    {
                        OrganizationDTO organization = JsonConvert.DeserializeObject<OrganizationDTO>(apiResponse.Result.ToString());
                        if (organization != null)
                        {
                            organization.IsDetailsAvailable = true;

                            return new ServiceResult(true, apiResponse.Message, organization);
                        }
                        else
                        {
                            return new ServiceResult(false, apiResponse.Message, organization);
                        }
                    }
                    else
                    {
                        _logger.LogError(apiResponse.Message);
                    }
                }
                else
                {
                    _logger.LogError($"The request with URI={response.RequestMessage.RequestUri} failed " +
                           $"with status code={response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new ServiceResult(false, "An error occurred while getting organization details. Please try later.");
        }

        public async Task<string[]> GetOrganizationNamesAndIdAysnc()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"api/get/organiztion");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    APIResponse apiResponse = JsonConvert.DeserializeObject<APIResponse>(await response.Content.ReadAsStringAsync());
                    if (apiResponse.Success)
                    {
                        return JsonConvert.DeserializeObject<string[]>(apiResponse.Result.ToString());
                    }
                    else
                    {
                        _logger.LogError(apiResponse.Message);
                    }
                }
                else
                {
                    _logger.LogError($"The request with uri={response.RequestMessage.RequestUri} failed " +
                       $"with status code={response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}


