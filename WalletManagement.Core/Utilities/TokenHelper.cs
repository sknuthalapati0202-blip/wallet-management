using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Utilities
{
    public class TokenHelper : ITokenHelper
    {
        private readonly ILogger<TokenHelper> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public TokenHelper(ILogger<TokenHelper> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ServiceResult> IntrospectToken(string token)
        {
            try
            {
                var introspectUrl = _configuration.GetValue<string>("DTIDP_Config:Introspect");
                var clientId = _configuration.GetValue<string>("DTIDP_Config:ClientId");
                var clientSecret = _configuration.GetValue<string>("DTIDP_Config:ClientSecret");

                var plainText = $"{clientId}:{clientSecret}";
                var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

                HttpClient client = _httpClientFactory.CreateClient("ignoreSSL");
                client.BaseAddress = new Uri(introspectUrl);

                client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Credentials}");

                var collection = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", token)
                };

                var content = new FormUrlEncodedContent(collection);

                var response = await client.PostAsync(introspectUrl, content);

                if (response == null)
                {
                    return new ServiceResult(false, "Introspect response is null");
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResult(false,
                        $"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                IntrospectResponse info = System.Text.Json.JsonSerializer.Deserialize<IntrospectResponse>(responseString, options);

                _logger.LogInformation("Token introspection successful for token: {Token}", token);
                _logger.LogInformation("Introspection response: {Response}", responseString);

                return new ServiceResult(true, "Token introspection successful", info);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured while calling introspect API - {error}", ex.ToString());
                return new ServiceResult(false, "Exception occured while calling introspect API");
            }
        }

        public async Task<JObject?> GetUserInfo(string accessToken)
        {
            try
            {
                var UserInfoUrl = _configuration.GetValue<string>(
                    "DTIDP_Config:UserInfo");

                HttpClient client = _httpClientFactory.CreateClient("ignoreSSL");
                client.BaseAddress = new Uri(UserInfoUrl);
                var authzHeader = "Bearer  " + accessToken;
                client.DefaultRequestHeaders.Add(_configuration["AccessTokenHeaderName"],
                    authzHeader);

                var response = await client.GetAsync(UserInfoUrl);
                if (response == null)
                {
                    throw new Exception("get user info responce getting null");
                }
                if (!response.IsSuccessStatusCode)
                {
                    dynamic error = new JObject();
                    error.error = response.StatusCode;
                    error.error_description = response.ReasonPhrase;
                    return error;
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject info = JObject.Parse(responseString);
                    return info;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured while getting user info - ", ex.ToString());
                return null;
            }
        }

        public async Task<ServiceResult> GetUserEmail(string suid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(suid))
                {
                    return new ServiceResult(false, "Invalid user id");
                }

                var userEmailUrl = _configuration.GetValue<string>("DTIDP_Config:UserEmail");

                var client = _httpClientFactory.CreateClient("ignoreSSL");

                var requestUrl = $"{userEmailUrl}?suid={suid}";

                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get user email. StatusCode: {StatusCode}", response.StatusCode);
                    return new ServiceResult(false, "Failed to get User Email");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                _logger.LogInformation("Received response for user email: {Response}", responseString);

                var result = System.Text.Json.JsonSerializer.Deserialize<APIResponse>(responseString, options);

                var serviceResult = new ServiceResult(result?.Success ?? false, result?.Message ?? "Failed to get User Email", result?.Result);

                return serviceResult ?? new ServiceResult(false, "Invalid response received");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user email for SUID: {Suid}", suid);
                return new ServiceResult(false, "Failed to get User Email");
            }
        }

    }
}
