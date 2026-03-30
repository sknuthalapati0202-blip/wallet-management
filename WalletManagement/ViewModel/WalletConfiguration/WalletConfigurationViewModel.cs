using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.ViewModel.WalletConfiguration
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class WalletConfigurationViewModel
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one credential format is required.")]
        [NoNullElements(ErrorMessage = "bindingMethods cannot contain null items.")]
        [JsonPropertyName("credentialFormats")]
        public List<CredentialFormats> CredentialFormats { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one binding method is required.")]
        [NoNullElements(ErrorMessage = "bindingMethods cannot contain null items.")]
        [JsonPropertyName("bindingMethods")]
        public List<BindingMethods> BindingMethods { get; set; }
    }
}