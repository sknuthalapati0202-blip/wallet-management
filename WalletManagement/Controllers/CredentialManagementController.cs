using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class CredentialManagementController : BaseApiController
    {
        private readonly ICredentialService _credentialService;
        private readonly ILogger<CredentialManagementController> _logger;
        private readonly IQrCredentialService _qrCredentialService;

        public CredentialManagementController(ICredentialService credentialService, ILogger<CredentialManagementController> logger, IQrCredentialService qrCredentialService)
        {
            _credentialService = credentialService;
            _logger = logger;
            _qrCredentialService = qrCredentialService;
        }

        [HttpGet("Activate/{credentialId:guid}")]
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

        [HttpPost("Reject")]
        [Consumes("application/json")]
        public async Task<IActionResult> RejectCredential([FromBody][Required] RejectCredentialDTO request)
        {
            var response = await _credentialService.RejectCredential(request.credentialId, request.remarks);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("CredentialList")]
        public async Task<IActionResult> GetCredentialList()
        {
            var response = await _credentialService.GetCredentialList();
            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("ActivateQR/{credentialId:guid}")]
        public async Task<IActionResult> ActivateQrCredential(Guid credentialId)
        {
            var response = await _qrCredentialService.ActivateCredential(credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("RejectQr")]
        [Consumes("application/json")]
        public async Task<IActionResult> RejectQrCredential([FromBody][Required] RejectCredentialDTO request)
        {
            var response = await _qrCredentialService.RejectCredential(request.credentialId, request.remarks);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("QrCredentialList")]
        public async Task<IActionResult> GetQrCredentialList()
        {
            var response = await _qrCredentialService.GetCredentialList();
            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("View/{credentialId:int:min(1)}")]
        public async Task<IActionResult> GetCredentialDetails(int credentialId)
        {
            var response = await _credentialService.GetCredentialById(credentialId);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpGet("ViewQr/{credentialId:int:min(1)}")]
        public async Task<IActionResult> GetQrCredentialDetails(int credentialId)
        {
            var response = await _qrCredentialService.GetCredentialById(credentialId);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }
    }
}