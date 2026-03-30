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
    public class CredentialVerifiersController : BaseApiController
    {
        private readonly ICredentialVerifiersService _credentialVerifiersService;
        private readonly IConfiguration Configuration;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly ILogger<CredentialVerifiersController> _logger;
        private readonly IMessageLocalizer _messageLocalizer;
        private readonly MessageConstants Constants;
        private readonly OIDCConstants OIDCConstants;

        public CredentialVerifiersController(
            ICredentialVerifiersService credentialVerifiersService,
            IConfiguration configuration,
            IGlobalConfiguration globalConfiguration,
            ILogger<CredentialVerifiersController> logger,
            IMessageLocalizer messageLocalizer)
        {
            _credentialVerifiersService = credentialVerifiersService;
            _logger = logger;
            _globalConfiguration = globalConfiguration;
            Configuration = configuration;
            _messageLocalizer = messageLocalizer;

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

        [Route("GetCredentialVerifiersList")]
        [HttpGet]
        public async Task<IActionResult> GetCredentialVerifiersList()
        {
            var response = await _credentialVerifiersService.GetCredentialVerifierDTOsListAsync();
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetActiveCredentialVerifiersList")]
        [HttpGet]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveCredentialVerifiersList()
        {
            var authHeaderName = Configuration["AccessTokenHeaderName"] ?? "Authorization";
            var authHeader = Request.Headers[authHeaderName];

            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken),
                    error_description = _messageLocalizer.GetMessage(OIDCConstants.InvalidToken)
                });
            }

            // Safely parse the authorization header
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

            var response = await _credentialVerifiersService.GetActiveCredentialVerifiersListAsync(authHeaderVal.Parameter);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetCredentialVerifiersListByOrganizationId/{orgId:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetCredentialVerifiersListByOrganizationId(Guid orgId)
        {
            var response = await _credentialVerifiersService.GetCredentialVerifiersListByOrganizationIdAsync(orgId.ToString());
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetActiveCredentialVerifiersListByOrganizationId/{orgId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveCredentialVerifiersListByOrganizationId(Guid orgId)
        {
            var authHeaderName = Configuration["AccessTokenHeaderName"] ?? "Authorization";
            var authHeader = Request.Headers[authHeaderName];

            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = "Invalid Token",
                    error_description = "Invalid Token"
                });
            }

            // Safely parse the authorization header
            if (!AuthenticationHeaderValue.TryParse(authHeader, out var authHeaderVal) ||
                string.IsNullOrEmpty(authHeaderVal.Scheme) ||
                string.IsNullOrEmpty(authHeaderVal.Parameter))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = "Invalid Token",
                    error_description = "Invalid Token"
                });
            }

            if (!authHeaderVal.Scheme.Equals("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new ErrorResponseDTO
                {
                    error = "Invalid Token",
                    error_description = "Invalid Token"
                });
            }

            var response = await _credentialVerifiersService.GetActiveCredentialVerifiersListByOrganizationIdAsync(orgId.ToString(), authHeaderVal.Parameter);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetCredentialVerifierById/{id:int:min(1)}")]
        [HttpGet]
        public async Task<IActionResult> GetCredentialVerifierById(int id)
        {
            var response = await _credentialVerifiersService.GetCredentialVerifierByIdAsync(id);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("CreateCredentialVerifier")]
        [HttpPost]
        public async Task<IActionResult> CreateCredentialVerifier([FromBody][Required] CredentialVerifierDTO credentialVerifierDTO)
        {
            var response = await _credentialVerifiersService.CreateCredentialVerifierAsync(credentialVerifierDTO);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("UpdateCredentialVerifier")]
        [HttpPost]
        public async Task<IActionResult> UpdateCredentialVerifier([FromBody][Required] CredentialVerifierDTO credentialVerifierDTO)
        {
            var response = await _credentialVerifiersService.UpdateCredentialVerifierAsync(credentialVerifierDTO);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetCredentialVerifierListByIssuerId/{orgId:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetCredentialVerifierListByIssuerId(Guid orgId)
        {
            var response = await _credentialVerifiersService.GetCredentialVerifierListByIssuerId(orgId.ToString());
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("ActivateCredential")]
        [HttpPost]
        public async Task<IActionResult> ActivateCredential([FromBody][Required] ActivateCredentialDTO activateCredentialDTO)
        {
            var response = await _credentialVerifiersService.ActivateCredentialById(activateCredentialDTO.Id);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("RejectCredential")]
        [HttpPost]
        public async Task<IActionResult> RejectCredential([FromBody][Required] ActivateCredentialDTO activateCredentialDTO)
        {
            var response = await _credentialVerifiersService.RejectCredentialById(activateCredentialDTO.Id, activateCredentialDTO.Remarks);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }
    }
}