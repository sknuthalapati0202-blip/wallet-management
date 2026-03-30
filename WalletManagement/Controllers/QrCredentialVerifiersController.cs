using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class QrCredentialVerifiersController : BaseApiController
    {
        private readonly IQrCredentialVerifiersService _qrCredentialVerifiersService;
        private readonly IConfiguration Configuration;

        public QrCredentialVerifiersController(
            IQrCredentialVerifiersService credentialVerifiersService,
            IConfiguration configuration)
        {
            _qrCredentialVerifiersService = credentialVerifiersService;
            Configuration = configuration;
        }

        [Route("GetQrCredentialVerifiersList")]
        [HttpGet]
        public async Task<IActionResult> GetQrCredentialVerifiersList()
        {
            var response = await _qrCredentialVerifiersService.GetQrCredentialVerifierDTOsListAsync();
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetQrCredentialVerifiersListByOrganizationId/{orgId:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetQrCredentialVerifiersListByOrganizationId(Guid orgId)
        {
            var response = await _qrCredentialVerifiersService.GetQrCredentialVerifiersListByOrganizationIdAsync(orgId.ToString());
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetActiveQrCredentialVerifiersListByOrganizationId/{orgId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveQrCredentialVerifiersListByOrganizationId(Guid orgId)
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

            var response = await _qrCredentialVerifiersService.GetActiveQrCredentialVerifiersListByOrganizationIdAsync(orgId.ToString(), authHeaderVal.Parameter);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("GetQrCredentialVerifierById/{id:int:min(1)}")]
        [HttpGet]
        public async Task<IActionResult> GetQrCredentialVerifierById(int id)
        {
            var response = await _qrCredentialVerifiersService.GetQrCredentialVerifierByIdAsync(id);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("CreateQrCredentialVerifier")]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateQrCredentialVerifier([FromBody][Required] QrCredentialVerifierDTO qrCredentialVerifierDTO)
        {
            var response = await _qrCredentialVerifiersService.CreateQrCredentialVerifierAsync(qrCredentialVerifierDTO);
            var result = new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            };
            return Ok(result);
        }

        [Route("UpdateQrCredentialVerifier")]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateQrCredentialVerifier([FromBody][Required] QrCredentialVerifierDTO qrCredentialVerifierDTO)
        {
            var response = await _qrCredentialVerifiersService.UpdateQrCredentialVerifierAsync(qrCredentialVerifierDTO);
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
            var response = await _qrCredentialVerifiersService.GetCredentialVerifierListByIssuerId(orgId.ToString());
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
        [Consumes("application/json")]
        public async Task<IActionResult> ActivateCredential([FromBody][Required] ActivateCredentialDTO activateCredentialDTO)
        {
            var response = await _qrCredentialVerifiersService.ActivateQrCredentialById(activateCredentialDTO.Id);

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
        [Consumes("application/json")]
        public async Task<IActionResult> RejectCredential([FromBody][Required] ActivateCredentialDTO activateCredentialDTO)
        {
            var response = await _qrCredentialVerifiersService.RejectQrCredentialById(activateCredentialDTO.Id, activateCredentialDTO.Remarks);

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