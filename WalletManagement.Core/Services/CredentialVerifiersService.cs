using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.Json;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;
using WalletManagement.Core.Utilities;

namespace WalletManagement.Core.Services
{
    public class CredentialVerifiersService : ICredentialVerifiersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CredentialVerifiersService> _logger;
        private readonly IOrganizationService _organizationService;
        private readonly ICredentialService _credentialService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHelper _helper;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly IMessageLocalizer _messageLocalizer;
        private readonly MessageConstants Constants;
        private readonly OIDCConstants OIDCConstants;
        private readonly WebConstants WebConstants;
        private readonly ITokenHelper _tokenHelper;

        public CredentialVerifiersService(IUnitOfWork unitOfWork,
            ILogger<CredentialVerifiersService> logger,
            IMessageLocalizer messageLocalizer,
            IGlobalConfiguration globalConfiguration,
            IOrganizationService organizationService,
            ICredentialService credentialService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ITokenHelper tokenHelper,
            IHelper helper)
        {
            _unitOfWork = unitOfWork;
            _messageLocalizer = messageLocalizer;
            _globalConfiguration = globalConfiguration;
            _logger = logger;
            _organizationService = organizationService;
            _credentialService = credentialService;
            _httpClientFactory = httpClientFactory;
            _helper = helper;
            _configuration = configuration;
            _tokenHelper = tokenHelper;

            var errorConfiguration = _globalConfiguration.
            GetErrorConfiguration();
            if (null == errorConfiguration)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }

            Constants = errorConfiguration.Constants;

            if (null == Constants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }

            OIDCConstants = errorConfiguration.OIDCConstants;
            if (null == OIDCConstants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }
            WebConstants = errorConfiguration.WebConstants;
            if (null == WebConstants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new NullReferenceException();
            }
        }
        public async Task<ServiceResult> GetCredentialVerifierDTOsListAsync()
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetAllAsync();
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _credentialService.GetCredentialNameIdListAsync();
                if (credential == null || !credential.Success)
                {
                    _logger.LogError("Creddential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var credentialDict = (Dictionary<string, string>)credential.Resource;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }

                var list = new List<CredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new CredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(item.Attributes),
                        emails = JsonConvert.DeserializeObject<List<string>>(item.Emails),
                        status = item.Status,
                        createdDate = item.CreatedDate,
                        updatedDate = item.UpdatedDate,
                    };
                    list.Add(credentialVerifierDTO);
                }
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", list);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> CreateCredentialVerifierAsync(CredentialVerifierDTO credentialVerifierDTO)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.IsCredentialAlreadyExists(credentialVerifierDTO);
                if (credentialVerifierinDb)
                {
                    _logger.LogError("Credential Verifier Already Exists");
                    return new ServiceResult(false, "Credential Verifier Already Exists");
                }

                var data = new CredentialVerifier()
                {
                    CredentialId = credentialVerifierDTO.credentialId,
                    OrganizationId = credentialVerifierDTO.organizationId,
                    Configuration = JsonConvert.SerializeObject(credentialVerifierDTO.configuration),
                    Attributes = JsonConvert.SerializeObject(credentialVerifierDTO.attributes),
                    Emails = JsonConvert.SerializeObject(credentialVerifierDTO.emails),
                    Validity = credentialVerifierDTO.validity,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    Status = "APPROVAL REQUIRED"
                };
                if (credentialVerifierDTO.domainConfig != null)
                {
                    data.Domains = JsonConvert.SerializeObject(credentialVerifierDTO.domainConfig);
                }
                await _unitOfWork.CredentialVerifiers.AddAsync(data);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Your Request Has Sent For Approval");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to create credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> UpdateCredentialVerifierAsync(CredentialVerifierDTO credentialVerifierDTO)
        {
            try
            {

                //var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.GetByIdAsync((int)credentialVerifierDTO.id);
                if (credentialVerifierDTO.id == null)
                {
                    _logger.LogError("QrCredential Verifier id is required for update");
                    return new ServiceResult(false, "QrCredential Verifier id is required for update");
                }
                if (credentialVerifierDTO.id.Value < 1 || credentialVerifierDTO.id.Value > int.MaxValue)
                {
                    return new ServiceResult(false, "Id is not a valid value.");
                }
                var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.GetByIdAsync((int)credentialVerifierDTO.id.Value);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }

                //var isCredentialExists = await _unitOfWork.CredentialVerifiers.IsCredentialAlreadyExists(credentialVerifierDTO);
                //if (isCredentialExists)
                //{
                //    _logger.LogError("Credential Verifier Already Exists");
                //    return new ServiceResult(false, "Credential Verifier Already Exists");
                //}

                credentialVerifierinDb.CredentialId = credentialVerifierDTO.credentialId;
                credentialVerifierinDb.OrganizationId = credentialVerifierDTO.organizationId;
                credentialVerifierinDb.Configuration = JsonConvert.SerializeObject(credentialVerifierDTO.configuration);
                credentialVerifierinDb.Attributes = JsonConvert.SerializeObject(credentialVerifierDTO.attributes);
                credentialVerifierinDb.Emails = JsonConvert.SerializeObject(credentialVerifierDTO.emails);
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = credentialVerifierDTO.status;


                _unitOfWork.CredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Updated Credential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetCredentialVerifierByIdAsync(int id)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("No Credential Verifier Data found");
                    return new ServiceResult(false, "No Credential Verifier Data found");
                }


                var credential = await _unitOfWork.Credential.GetCredentialByUidAsync(credentialVerifierinDb.CredentialId);
                if (credential == null)
                {
                    _logger.LogError("Creddential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var CredentialName = credential.DisplayName;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }
                var OrganizationName = organizationDict[credentialVerifierinDb.OrganizationId];

                var credentialVerifierDTO = new CredentialVerifierDTO()
                {
                    id = id,
                    credentialId = credentialVerifierinDb.CredentialId,
                    credentialName = CredentialName,
                    organizationId = credentialVerifierinDb.OrganizationId,
                    organizationName = OrganizationName,
                    configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(credentialVerifierinDb.Configuration),
                    attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(credentialVerifierinDb.Attributes),
                    emails = JsonConvert.DeserializeObject<List<string>>(credentialVerifierinDb.Emails),
                    status = credentialVerifierinDb.Status,
                    remarks = credentialVerifierinDb.Remarks,
                    createdDate = credentialVerifierinDb.CreatedDate,
                    updatedDate = credentialVerifierinDb.UpdatedDate,
                };
                if (credentialVerifierinDb.Domains != null)
                {
                    credentialVerifierDTO.domainConfig = JsonConvert.DeserializeObject<DomainConfig>(credentialVerifierinDb.Domains);
                }
                if (credentialVerifierinDb.Validity != null)
                {
                    credentialVerifierDTO.validity = (int)credentialVerifierinDb.Validity;
                }
                return new ServiceResult(true, "Successfully got Credential verifier", credentialVerifierDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetCredentialVerifiersListByOrganizationIdAsync(string organizationId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetCredentialListDataByOrganizationIdAsync(organizationId);
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _credentialService.GetCredentialNameIdListAsync();
                if (credential == null || !credential.Success)
                {
                    _logger.LogError("Creddential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var credentialDict = (Dictionary<string, string>)credential.Resource;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }

                var list = new List<CredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new CredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(item.Attributes),
                        emails = JsonConvert.DeserializeObject<List<string>>(item.Emails),
                        status = item.Status,
                        createdDate = item.CreatedDate,
                        updatedDate = item.UpdatedDate,
                    };
                    list.Add(credentialVerifierDTO);
                }
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", list);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetActiveCredentialVerifiersListAsync(string accessToken)
        {
            try
            {
                var introspectResult = await _tokenHelper.IntrospectToken(accessToken);
                if (!introspectResult.Success)
                {
                    _logger.LogError("Token introspection failed: " + introspectResult.Message);
                    return new ServiceResult(false, _messageLocalizer.GetMessage(Constants.UnAuthorized));
                }

                var introspectData = (IntrospectResponse)introspectResult.Resource;
                if (introspectData == null || !introspectData.active)
                {
                    _logger.LogError("Token is not active");
                    return new ServiceResult(false, _messageLocalizer.GetMessage(Constants.UnAuthorized));
                }

                string organizationId = introspectData.org_id;

                if (string.IsNullOrEmpty(organizationId))
                {
                    _logger.LogError("Organization ID not found in token introspection data");
                    return new ServiceResult(false, _messageLocalizer.GetMessage(Constants.UnAuthorized));
                }

                var emailData = await _tokenHelper.GetUserEmail(introspectData.username);

                if (emailData == null || !emailData.Success)
                {
                    _logger.LogError("Getting Email Failed");
                    return new ServiceResult(false, _messageLocalizer.GetMessage(WebConstants.EmailNotFoundInUserInfo));
                }

                string subscriberEmail = "";

                if (emailData.Resource is JsonElement element)
                {
                    if (element.TryGetProperty("email", out JsonElement emailElement))
                    {
                        subscriberEmail = emailElement.GetString();
                    }
                }

                if (string.IsNullOrEmpty(subscriberEmail))
                {
                    _logger.LogError("Email claim not found in user info");
                    return new ServiceResult(false, _messageLocalizer.GetMessage(WebConstants.EmailNotFoundInUserInfo));
                }


                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetActiveCredentialVerifierListAsync();
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, _messageLocalizer.GetMessage(WebConstants.NoCredentialVerifiersDataFound));
                }

                var list = new List<CredentialVerifiersListDTO>();

                foreach (var item in credentialVerifiersListInDb)
                {
                    _logger.LogDebug(
                        "Processing CredentialVerifier. CredentialId: {CredentialId}, OrganizationId: {OrganizationId}",
                        item.CredentialId,
                        item.OrganizationId
                    );

                    var mails = new List<string>();

                    if (!string.IsNullOrEmpty(item.Emails))
                    {
                        mails = JsonConvert.DeserializeObject<List<string>>(item.Emails);

                        _logger.LogDebug(
                            "Deserialized Emails for CredentialId {CredentialId}: {Emails}",
                            item.CredentialId,
                            JsonConvert.SerializeObject(mails)
                        );
                    }

                    bool isEmailMatched = mails.Any(mail =>
                        mail.Equals(subscriberEmail, StringComparison.OrdinalIgnoreCase));

                    if (item.OrganizationId == organizationId && isEmailMatched)
                    {
                        _logger.LogInformation(
                            "Email matched for CredentialId {CredentialId}. Adding to response list.",
                            item.CredentialId
                        );

                        var credentialVerifierDTO = new CredentialVerifiersListDTO
                        {
                            credentialName = item.Credential.CredentialName,
                            displayName = item.Credential.DisplayName,
                            credentialId = item.CredentialId,
                            organizationId = item.OrganizationId,
                            logo = item.Credential.Logo,
                            attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(item.Attributes)
                        };

                        list.Add(credentialVerifierDTO);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "Skipped CredentialId {CredentialId}. OrgMatch: {OrgMatch}",
                            item.CredentialId,
                            item.OrganizationId == organizationId
                        );
                    }
                }

                _logger.LogInformation(
                    "Total Credential Verifiers found: {Count}",
                    list.Count
                );

                // Print full list (use Debug to avoid noisy production logs)
                _logger.LogDebug(
                    "Credential Verifiers List Response: {CredentialVerifiersList}",
                    JsonConvert.SerializeObject(list)
                );

                return new ServiceResult(true, _messageLocalizer.GetMessage(WebConstants.CredentialVerifiersListSuccess), list);

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, _messageLocalizer.GetMessage(WebConstants.CredentialVerifierListFailed) + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetActiveCredentialVerifiersListByOrganizationIdAsync(string orgId, string token)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetActiveCredentialListByOrganizationIdAsync(orgId);
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _credentialService.GetCredentialNameIdListAsync();
                if (credential == null || !credential.Success)
                {
                    _logger.LogError("Creddential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var credentialDict = (Dictionary<string, string>)credential.Resource;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }

                var list = new List<CredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new CredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(item.Attributes),
                        emails = JsonConvert.DeserializeObject<List<string>>(item.Emails),
                        status = item.Status,
                        createdDate = item.CreatedDate,
                        updatedDate = item.UpdatedDate,
                    };
                    list.Add(credentialVerifierDTO);
                }
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", list);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetCredentialsListByOrganizationId(string organizationId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetCredentialsListByOrganizationIdAsync(organizationId);
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", credentialVerifiersListInDb);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> ActivateCredentialById(int id)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = "SUBSCRIBED";


                _unitOfWork.CredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Activated Credential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> RejectCredentialById(int id, string remarks)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.CredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = "REJECTED";
                credentialVerifierinDb.Remarks = remarks;


                _unitOfWork.CredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Updated Credential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<Dictionary<string, string>?> GetOrganizationsDictionary()
        {
            try
            {
                var dict = new Dictionary<string, string>();

                var organizationsList = await _organizationService.GetOrganizationNamesAndIdAysnc();

                foreach (var organization in organizationsList)
                {
                    var data = organization.Split(',');
                    dict[data[1]] = data[0];
                }
                return dict;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ServiceResult> GetCredentialVerifierListByIssuerId(string orgId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.CredentialVerifiers.GetCredentialVerifierListByIssuerIdAsync(orgId);
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }
                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }

                var list = new List<CredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new CredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = item.Credential.CredentialName,
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.Credential.OrganizationId],
                        configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<List<DataAttributes>>(item.Attributes),
                        emails = JsonConvert.DeserializeObject<List<string>>(item.Emails),
                        status = item.Status,
                        createdDate = item.CreatedDate,
                        updatedDate = item.UpdatedDate,
                    };
                    list.Add(credentialVerifierDTO);
                }
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", list);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }
    }
}
