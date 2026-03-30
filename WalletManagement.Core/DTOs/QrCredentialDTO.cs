using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class QrCredentialDTO
    {
        public long? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "credentialName cannot be only whitespace or contain control characters.")]
        public string credentialName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
     ErrorMessage = "displayName cannot be whitespace or contain control characters.")]
        public string displayName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string credentialUId { get; set; }

        [StringLength(500)]
        public string? remarks { get; set; }

        [Required]
        public QrAttributesDTO dataAttributes { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string organizationId { get; set; }

        public string? credentialOffer { get; set; }

        [JsonIgnore]
        public DateTime? createdDate { get; set; }

        public bool? portraitVerificationRequired { get; set; }

        public string? status { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class QrAttributesDTO
    {
        [Required(ErrorMessage = "publicAttributes is required.")]
        [NoNullElements]
        public List<DataAttributesDTO> publicAttributes { get; set; }
        [Required(ErrorMessage = "privateAttributes is required.")]
        [NoNullElements]
        public List<DataAttributesDTO> privateAttributes { get; set; }
    }
}