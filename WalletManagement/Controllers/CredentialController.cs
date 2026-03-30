using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;
using WalletManagement.Core.Utilities;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class CredentialController : BaseApiController
    {
        private readonly ICredentialService _credentialService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CredentialController> _logger;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly IMessageLocalizer _messageLocalizer;
        private readonly MessageConstants Constants;
        private readonly OIDCConstants OIDCConstants;

        public CredentialController(ICredentialService credentialService,
            IGlobalConfiguration globalConfiguration,
            IMessageLocalizer messageLocalizer,
            IConfiguration configuration, ILogger<CredentialController> logger)
        {
            _credentialService = credentialService;
            _configuration = configuration;
            _messageLocalizer = messageLocalizer;
            _globalConfiguration = globalConfiguration;
            _logger = logger;

            var errorConfiguration = _globalConfiguration.GetErrorConfiguration();
            if (null == errorConfiguration)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(errorConfiguration));
            }

            Constants = errorConfiguration.Constants;
            if (null == Constants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(Constants));
            }

            OIDCConstants = errorConfiguration.OIDCConstants;
            if (null == OIDCConstants)
            {
                _logger.LogError("Get Error Configuration failed");
                throw new ArgumentNullException(nameof(OIDCConstants));
            }
        }

        [HttpGet("GetCredentialList")]
        public async Task<IActionResult> GetCredentialList()
        {
            var response = await _credentialService.GetCredentialList();

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpGet("GetActiveCredentialList")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveCredentialList()
        {
            var authHeaderName = _configuration["AccessTokenHeaderName"] ?? "Authorization";
            var authHeader = Request.Headers[authHeaderName];

            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            // Parse the authorization header safely
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authHeaderVal) ||
                string.IsNullOrEmpty(authHeaderVal.Scheme) ||
                string.IsNullOrEmpty(authHeaderVal.Parameter))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            // Check the authorization is of Bearer type
            if (!authHeaderVal.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            var response = await _credentialService.GetActiveCredentialList(authHeaderVal.Parameter);

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpGet("GetCredentialListByOrgUid")]
        public async Task<IActionResult> GetCredentialListByOrgUid(Guid orgUid)
        {
            var response = await _credentialService.GetCredentialListByOrgId(orgUid.ToString());

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpGet("GetCredentialById/{Id:int:min(1)}")]
        public async Task<IActionResult> GetCredentialById(int Id)
        {
            var response = await _credentialService.GetCredentialById(Id);

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpGet("GetCredentialByUid/{Id:guid}")]
        public async Task<IActionResult> GetCredentialByUid(Guid Id)
        {
            // Note: The previous code had {Id:int:min(1)} but accepted a 'string'. 
            // Changed route to just {Id} to prevent routing mismatch.
            var response = await _credentialService.GetCredentialByUid(Id.ToString());

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpGet("GetCredentialOfferByUid/{Id:guid}")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCredentialOfferByUid(Guid Id)
        {
            var authHeaderName = _configuration["AccessTokenHeaderName"] ?? "Authorization";
            var authHeader = Request.Headers[authHeaderName];

            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            // Parse the authorization header safely
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authHeaderVal) ||
                string.IsNullOrEmpty(authHeaderVal.Scheme) ||
                string.IsNullOrEmpty(authHeaderVal.Parameter))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            // Check the authorization is of Bearer type
            if (!authHeaderVal.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            var response = await _credentialService.GetCredentialOfferByUid(Id.ToString(), authHeaderVal.Parameter);

            var apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpPost("CreateCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCredential([FromBody][Required] CredentialDTO credentialDto)
        {
            var response = await _credentialService.CreateCredentialAsync(credentialDto);

            APIResponse apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };

            return Ok(apiResponse);
        }

        [HttpPost("UpdateCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCredential([FromBody][Required] CredentialDTO credentialDto)
        {
            var response = await _credentialService.UpdateCredential(credentialDto);
            APIResponse apiResponse = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = null
            };
            return Ok(apiResponse);
        }

        [HttpPost("TestCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> TestCredential([FromBody][Required] TestCredentialRequest testCredentialRequest)
        {
            var response = await _credentialService.TestCredential(testCredentialRequest.userId, testCredentialRequest.credentialId);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("ActivateCredential/{credentialId:guid}")]
        public async Task<IActionResult> ActivateCredential(Guid credentialId)
        {
            var response = await _credentialService.ActivateCredential(credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialDetails/{credentialId:guid}")]
        public async Task<IActionResult> GetCredentialDetails(Guid credentialId)
        {
            var response = await _credentialService.GetCredentialDetails(credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialNameIdList/{credentialId:guid}")]
        public async Task<IActionResult> GetCredentialNameIdList(Guid credentialId)
        {
            var response = await _credentialService.GetCredentialNameIdListAsync(credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialNameIdListAsync")]
        public async Task<IActionResult> GetCredentialNameIdListAsync()
        {
            var response = await _credentialService.GetCredentialNameIdListAsync();
            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetVerifiableCredentialList")]
        public async Task<IActionResult> GetVerifiableCredentialList(Guid orgId)
        {
            var response = await _credentialService.GetVerifiableCredentialList(orgId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetAuthSchemeList")]
        public IActionResult GetAuthSchemeList()
        {
            var authSchemeList = _configuration.GetSection("auth_schemes_supported").Get<string[]>() ?? Array.Empty<string>();

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var authScheme in authSchemeList)
            {
                dict[authScheme] = authScheme;
            }

            return Ok(new APIResponse()
            {
                Success = true,
                Message = "Get Auth Scheme List Success",
                Result = dict
            });
        }

        [HttpPost("SendToApproval")]
        [Consumes("application/json")]
        public async Task<IActionResult> SendToApproval([FromBody][Required] ApprovalRequest approvalRequest)
        {
            if (approvalRequest == null)
                return BadRequest(new APIResponse { Success = false, Message = "Request body is required." });
            var response = await _credentialService.SendToApproval(approvalRequest.credentialId, approvalRequest.signedDocument);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetAttributes")]
        public IActionResult GetAttributes()
        {
            string[] attributes = new string[] { "fullName", "dateOfBirth", "gender", "nationality", "mobileNumber", "email", "address", "idDocNumber", "pidIssueDate", "pidExpiryDate", "photo", "pidDocument", "cardNumber" };
            string[] displayNames = new string[] { "Full Name", "Date Of Birth", "Gender", "Nationality", "Mobile Number", "Email", "Address", "Id Document Number", "PID Issue Date", "PID Expiry Date", "Photo", "PID Document", "Card Number" };
            int[] type = new int[] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 1, 1, 4 };

            List<DataAttributesDTO> dataAttributesDTOs = new List<DataAttributesDTO>();
            for (int i = 0; i < attributes.Length; i++)
            {
                dataAttributesDTOs.Add(new DataAttributesDTO()
                {
                    attribute = attributes[i],
                    dataType = type[i],
                    displayName = displayNames[i],
                });
            }
            return Ok(new APIResponse()
            {
                Success = true,
                Message = "Get Attributes Success",
                Result = dataAttributesDTOs
            });
        }
    }
}