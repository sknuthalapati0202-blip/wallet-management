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
    public class QrCredentialController : BaseApiController
    {
        private readonly IQrCredentialService _qrCredentialService;
        private readonly IConfiguration _configuration;
        private readonly MessageConstants Constants;
        private readonly OIDCConstants OIDCConstants;
        private readonly IMessageLocalizer _messageLocalizer;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly ILogger<QrCredentialController> _logger;

        public QrCredentialController(
            IQrCredentialService qrCredentialService,
            IConfiguration configuration,
            IGlobalConfiguration globalConfiguration,
            IMessageLocalizer messageLocalizer,
            ILogger<QrCredentialController> logger)
        {
            _qrCredentialService = qrCredentialService;
            _configuration = configuration;
            _globalConfiguration = globalConfiguration;
            _messageLocalizer = messageLocalizer;
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
            var response = await _qrCredentialService.GetCredentialList();

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetActiveCredentialList")]
        public async Task<IActionResult> GetActiveCredentialList()
        {
            var response = await _qrCredentialService.GetActiveCredentialList();

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialListByOrgUid")]
        public async Task<IActionResult> GetCredentialListByOrgUid([FromQuery][Required] Guid orgUid)
        {
            var response = await _qrCredentialService.GetCredentialListByOrgId(orgUid.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialById/{Id:int:min(1)}")]
        public async Task<IActionResult> GetCredentialById(int Id)
        {
            var response = await _qrCredentialService.GetCredentialById(Id);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetCredentialByUid/{Id:guid}")]
        public async Task<IActionResult> GetCredentialByUid(Guid Id)
        {
            var response = await _qrCredentialService.GetCredentialByUid(Id.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetQrCredentialOfferByUid/{Id:guid}")]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetQrCredentialOfferByUid(Guid Id)
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

            var response = await _qrCredentialService.GetCredentialOfferByUid(Id.ToString(), authHeaderVal.Parameter);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("CreateCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCredential([FromBody][Required] QrCredentialDTO credentialDto)
        {
            var response = await _qrCredentialService.CreateCredentialAsync(credentialDto);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("UpdateCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCredential([FromBody][Required] QrCredentialDTO credentialDto)
        {
            var response = await _qrCredentialService.UpdateCredential(credentialDto);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = null
            });
        }

        [HttpPost("TestCredential")]
        [Consumes("application/json")]
        public async Task<IActionResult> TestCredential([FromBody][Required] QrTestCredentialRequest testCredentialRequest)
        {
            var response = await _qrCredentialService.TestCredential(testCredentialRequest);

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
            var response = await _qrCredentialService.ActivateCredential(credentialId.ToString());

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
            var response = await _qrCredentialService.GetCredentialDetails(credentialId.ToString());

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
            var response = await _qrCredentialService.GetCredentialNameIdListAsync(credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("GetVerifiableCredentialList/{orgId:guid}")]
        public async Task<IActionResult> GetVerifiableCredentialList(Guid orgId)
        {
            var response = await _qrCredentialService.GetVerifiableCredentialList(orgId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("SendToApproval")]
        [Consumes("application/json")]
        public async Task<IActionResult> SendToApproval([FromBody][Required] ApprovalRequest approvalRequest)
        {
            var response = await _qrCredentialService.SendToApproval(approvalRequest.credentialId, approvalRequest.signedDocument);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }
    }
}