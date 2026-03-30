using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.ViewModel.WalletConfiguration;

namespace WalletManagement.Controllers
{
    [Route("api/[controller]")]
    public class WalletConfigurationsController : BaseApiController
    {
        private readonly IWalletConfigurationService _walletConfigurationService;

        public WalletConfigurationsController(IWalletConfigurationService walletConfigurationService)
        {
            _walletConfigurationService = walletConfigurationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletConfiguration()
        {
            var response = await _walletConfigurationService.GetConfiguration();
            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }

        [HttpPost("Update")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update([FromBody][Required] WalletConfigurationViewModel model)
        {
            if (model == null ||
                (model.CredentialFormats == null || !model.CredentialFormats.Any()) &&
                (model.BindingMethods == null || !model.BindingMethods.Any()))
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "At least one configuration must be provided."
                });
            }

            WalletConfigurationResponse walletconfig = new WalletConfigurationResponse()
            {
                BindingMethods = model.BindingMethods ?? new List<BindingMethods>(),
                CredentialFormats = model.CredentialFormats ?? new List<CredentialFormats>(),
            };
            var response = await _walletConfigurationService.UpdateWalletConfiguration(walletconfig);

            return Ok(new APIResponse()
            {
                Success = response.Success,
                Message = response.Message,
                Result = response.Resource
            });
        }
    }
}