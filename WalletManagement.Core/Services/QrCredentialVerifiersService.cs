using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;
using WalletManagement.Core.Utilities;

namespace WalletManagement.Core.Services
{
    public class QrCredentialVerifiersService : IQrCredentialVerifiersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QrCredentialVerifiersService> _logger;
        private readonly IOrganizationService _organizationService;
        private readonly IQrCredentialService _qrCredentialService;
        private readonly IHelper _helper;
        private readonly ITokenHelper _tokenHelper;
        private readonly IMessageLocalizer _messageLocalizer;
        public QrCredentialVerifiersService(IUnitOfWork unitOfWork,
            ILogger<QrCredentialVerifiersService> logger,
            IOrganizationService organizationService,
            IQrCredentialService qrCredentialService,
            ITokenHelper tokenHelper,
            IMessageLocalizer messageLocalizer,
            IHelper helper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _organizationService = organizationService;
            _qrCredentialService = qrCredentialService;
            _helper = helper;
            _tokenHelper = tokenHelper;
            _messageLocalizer = messageLocalizer;
        }
        public async Task<ServiceResult> GetQrCredentialVerifierDTOsListAsync()
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.QrCredentialVerifiers.GetAllAsync();
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _qrCredentialService.GetCredentialNameIdListAsync();
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

                var list = new List<QrCredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new QrCredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        //configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<QrAttributesDTO>(item.Attributes),
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

        public async Task<ServiceResult> CreateQrCredentialVerifierAsync(QrCredentialVerifierDTO qrCredentialVerifierDTO)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.IsCredentialAlreadyExists(qrCredentialVerifierDTO);
                if (credentialVerifierinDb)
                {
                    _logger.LogError("Credential Verifier Already Exists");
                    return new ServiceResult(false, "Credential Verifier Already Exists");
                }

                var data = new QrCredentialVerifier()
                {
                    CredentialId = qrCredentialVerifierDTO.credentialId,
                    OrganizationId = qrCredentialVerifierDTO.organizationId,
                    //Configuration = JsonConvert.SerializeObject(qrCredentialVerifierDTO.configuration),
                    Attributes = JsonConvert.SerializeObject(qrCredentialVerifierDTO.attributes),
                    Emails = JsonConvert.SerializeObject(qrCredentialVerifierDTO.emails),
                    CreatedDate = DateTime.UtcNow,
                    Status = "APPROVAL REQUIRED"
                };

                await _unitOfWork.QrCredentialVerifiers.AddAsync(data);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Your Request has Sent for Approval");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to create credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> UpdateQrCredentialVerifierAsync(QrCredentialVerifierDTO qrCredentialVerifierDTO)
        {
            try
            {
                //var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync((int)qrCredentialVerifierDTO.id);
                if (qrCredentialVerifierDTO.id == null)
                {
                    _logger.LogError("QrCredential Verifier id is required for update");
                    return new ServiceResult(false, "QrCredential Verifier id is required for update");
                }
                if (qrCredentialVerifierDTO.id.Value < 1 || qrCredentialVerifierDTO.id.Value > int.MaxValue)
                {
                    return new ServiceResult(false, "Id is not a valid value.");
                }
                var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync((int)qrCredentialVerifierDTO.id.Value);
                // var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync(qrCredentialVerifierDTO.id.Value);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }

                //var isCredentialExists = await _unitOfWork.QrCredentialVerifiers.IsCredentialAlreadyExists(qrCredentialVerifierDTO);
                //if (isCredentialExists)
                //{
                //    _logger.LogError("Credential Verifier Already Exists");
                //    return new ServiceResult(false, "Credential Verifier Already Exists");
                //}

                credentialVerifierinDb.CredentialId = qrCredentialVerifierDTO.credentialId;
                credentialVerifierinDb.OrganizationId = qrCredentialVerifierDTO.organizationId;
                //credentialVerifierinDb.Configuration = JsonConvert.SerializeObject(qrCredentialVerifierDTO.configuration);
                credentialVerifierinDb.Attributes = JsonConvert.SerializeObject(qrCredentialVerifierDTO.attributes);
                credentialVerifierinDb.Emails = JsonConvert.SerializeObject(qrCredentialVerifierDTO.emails);
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = qrCredentialVerifierDTO.status;


                _unitOfWork.QrCredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Updated Credential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetQrCredentialVerifierByIdAsync(int id)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("No Credential Verifier Data found");
                    return new ServiceResult(false, "No Credential Verifier Data found");
                }


                var credential = await _unitOfWork.QrCredential.GetCredentialByUidAsync(credentialVerifierinDb.CredentialId);
                if (credential == null)
                {
                    _logger.LogError("Creddential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var CredentialName = credential.CredentialName;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }
                var OrganizationName = organizationDict[credentialVerifierinDb.OrganizationId];

                var credentialVerifierDTO = new QrCredentialVerifierDTO()
                {
                    id = id,
                    credentialId = credentialVerifierinDb.CredentialId,
                    credentialName = CredentialName,
                    organizationId = credentialVerifierinDb.OrganizationId,
                    organizationName = OrganizationName,
                    attributes = JsonConvert.DeserializeObject<QrAttributesDTO>(credentialVerifierinDb.Attributes),
                    emails = JsonConvert.DeserializeObject<List<string>>(credentialVerifierinDb.Emails),
                    status = credentialVerifierinDb.Status,
                    createdDate = credentialVerifierinDb.CreatedDate,
                    updatedDate = credentialVerifierinDb.UpdatedDate
                };

                return new ServiceResult(true, "Successfully got Credential verifier", credentialVerifierDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetQrCredentialVerifiersListByOrganizationIdAsync(string organizationId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.QrCredentialVerifiers.GetCredentialListDataByOrganizationIdAsync(organizationId);
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _qrCredentialService.GetCredentialNameIdListAsync();
                if (credential == null || !credential.Success)
                {
                    _logger.LogError("Credential Data found");
                    return new ServiceResult(false, "Credential Data found");
                }
                var credentialDict = (Dictionary<string, string>)credential.Resource;


                var organizationDict = await GetOrganizationsDictionary();
                if (organizationDict == null)
                {
                    _logger.LogError("GetOrganizationsDictionary failed");
                    return new ServiceResult(false, "Get Organizations Data found");
                }

                var list = new List<QrCredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new QrCredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        //configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<QrAttributesDTO>(item.Attributes),
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

        public async Task<ServiceResult> GetActiveQrCredentialVerifiersListByOrganizationIdAsync(string orgId, string token)
        {
            try
            {
                var introspectResult = await _tokenHelper.IntrospectToken(token);
                if (!introspectResult.Success)
                {
                    _logger.LogError("Token introspection failed: " + introspectResult.Message);
                    return new ServiceResult(false, "Token introspect failed");
                }

                var introspectData = (IntrospectResponse)introspectResult.Resource;
                if (introspectData == null || !introspectData.active)
                {
                    _logger.LogError("Token is not active");
                    return new ServiceResult(false, "Token is not active");
                }

                var credentialVerifiersListInDb = await _unitOfWork.QrCredentialVerifiers.GetActiveCredentialListByOrganizationIdAsync(orgId);
                if (credentialVerifiersListInDb == null)
                {
                    _logger.LogError("No Credential Verifiers Data found");
                    return new ServiceResult(false, "No Credential Verifiers Data found");
                }

                var credential = await _qrCredentialService.GetCredentialNameIdListAsync();
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

                var list = new List<QrCredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new QrCredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = credentialDict[item.CredentialId],
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.OrganizationId],
                        //configuration = JsonConvert.DeserializeObject<List<CredentialConfig>>(item.Configuration),
                        attributes = JsonConvert.DeserializeObject<QrAttributesDTO>(item.Attributes),
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

        public async Task<ServiceResult> GetQrCredentialsListByOrganizationId(string organizationId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.QrCredentialVerifiers.GetCredentialsListByOrganizationIdAsync(organizationId);
                return new ServiceResult(true, "Successfully recieved Credential verifiers list", credentialVerifiersListInDb);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get credential verifier list : " + ex.ToString());
                return new ServiceResult(false, "Failed to get credential verifier list : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> GetCredentialVerifierListByIssuerId(string orgId)
        {
            try
            {
                var credentialVerifiersListInDb = await _unitOfWork.QrCredentialVerifiers.GetCredentialVerifierListByIssuerIdAsync(orgId);
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

                var list = new List<QrCredentialVerifierDTO>();
                foreach (var item in credentialVerifiersListInDb)
                {
                    var credentialVerifierDTO = new QrCredentialVerifierDTO()
                    {
                        id = item.Id,
                        credentialId = item.CredentialId,
                        credentialName = item.Credential.CredentialName,
                        organizationId = item.OrganizationId,
                        organizationName = organizationDict[item.Credential.OrganizationId],
                        attributes = JsonConvert.DeserializeObject<QrAttributesDTO>(item.Attributes),
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

        public async Task<ServiceResult> ActivateQrCredentialById(int id)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = "SUBSCRIBED";


                _unitOfWork.QrCredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Activated QrCredential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<ServiceResult> RejectQrCredentialById(int id, string remarks)
        {
            try
            {
                var credentialVerifierinDb = await _unitOfWork.QrCredentialVerifiers.GetByIdAsync(id);
                if (credentialVerifierinDb == null)
                {
                    _logger.LogError("Failed to get Credential Verifer Data");
                    return new ServiceResult(false, "Failed to get Credential Verifer Data");
                }
                credentialVerifierinDb.UpdatedDate = DateTime.UtcNow;
                credentialVerifierinDb.Status = "REJECTED";
                credentialVerifierinDb.Remarks = remarks;


                _unitOfWork.QrCredentialVerifiers.Update(credentialVerifierinDb);

                await _unitOfWork.SaveAsync();

                return new ServiceResult(true, "Successfully Rejected QrCredential verifier");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update credential verifier : " + ex.ToString());
                return new ServiceResult(false, "Failed to update credential verifier : " + ex.ToString());
            }
        }

        public async Task<Dictionary<string, string>> GetOrganizationsDictionary()
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
    }
}
