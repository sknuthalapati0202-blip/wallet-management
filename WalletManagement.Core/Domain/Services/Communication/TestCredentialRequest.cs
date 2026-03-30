using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.Domain.Services.Communication
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class TestCredentialRequest
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string credentialId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string userId { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class QrTestCredentialRequest
    {
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string credentialId { get; set; }

        // Removed [Required] so API accepts null and returns 200 {success: false}
        public object? data { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class ApprovalRequest
    {
        // FIX: Tightened UUID regex — old pattern allowed dashes in any position
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(
            @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            ErrorMessage = "credentialId must be a valid UUID.")]
        public string credentialId { get; set; }

        // FIX: Added MinLength(1) and AllowEmptyStrings = false —
        // bare [Required] allows empty string "" which the backend rejects
        [Required(AllowEmptyStrings = false)]
        [MinLength(1, ErrorMessage = "signedDocument cannot be empty.")]
        public string signedDocument { get; set; }
    }
}