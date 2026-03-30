using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class QrCredentialVerifierDTO
    {
        public long? id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string credentialId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string organizationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "credentialName cannot be only whitespace or contain control characters.")]
        public string credentialName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "organizationName cannot be only whitespace or contain control characters.")]
        public string organizationName { get; set; }

        [Required]
        public QrAttributesDTO attributes { get; set; }

        [NoNullElements]
        public List<string>? emails { get; set; }

        public string? status { get; set; }

        [JsonIgnore]
        public DateTime? createdDate { get; set; }

        [JsonIgnore]
        public DateTime? updatedDate { get; set; }

        [StringLength(500)]
        public string? remarks { get; set; }
    }
}