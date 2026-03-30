using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class ProvisionStatusDTO
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Suid is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Suid must be at least 3 characters.")]
        public string Suid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CredentialId is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "CredentialId must be at least 3 characters.")]
        public string CredentialId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "DocumentId is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "DocumentId must be at least 3 characters.")]
        public string DocumentId { get; set; }
        public string? Status { get; set; }
    }
}