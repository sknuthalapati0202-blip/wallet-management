using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class RejectCredentialDTO
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(
            @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "credentialId must be a valid UUID.")]
        public string credentialId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Remarks field is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Remarks cannot be empty.")]
        public string remarks { get; set; }
    }
}