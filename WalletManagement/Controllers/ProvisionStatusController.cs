using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class ProvisionStatusController : BaseApiController
    {
        private readonly IProvisionStatusService _provisionStatusService;

        public ProvisionStatusController(IProvisionStatusService provisionStatusService)
        {
            _provisionStatusService = provisionStatusService;
        }

        [HttpGet("GetProvisionStatus")]
        // FIX: Moved validation to the parameters so [ApiController] can generate standard 400 responses.
        public async Task<IActionResult> GetProvisionStatus(
            [FromQuery, Required(ErrorMessage = "suid is required")] Guid suid,
            [FromQuery, Required(ErrorMessage = "credentialId is required")] Guid credentialId)
        {
            var response = await _provisionStatusService.GetProvisionStatus(suid.ToString(), credentialId.ToString());

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("AddProvisionStatus")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddProvisionStatus([FromBody][Required] ProvisionStatusDTO provisionStatusDTO)
        {
            var response = await _provisionStatusService.AddProvisionStatus(
                provisionStatusDTO.Suid,
                provisionStatusDTO.CredentialId,
                provisionStatusDTO.Status,
                provisionStatusDTO.DocumentId);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("RevokeProvision")]
        [Consumes("application/json")]
        public async Task<IActionResult> RevokeProvision([FromBody][Required] ProvisionStatusDTO provisionStatusDTO)
        {
            var response = await _provisionStatusService.RevokeProvision(
                provisionStatusDTO.CredentialId,
                provisionStatusDTO.DocumentId);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("DeleteProvision")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteProvision([FromBody][Required] ProvisionStatusDTO provisionStatusDTO)
        {
            var response = await _provisionStatusService.DeleteProvision(
                provisionStatusDTO.CredentialId,
                provisionStatusDTO.Suid);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }
    }
}