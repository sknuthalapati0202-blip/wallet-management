using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CredentialDTO
    {
        public long? Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "credentialName cannot be only whitespace or contain control characters.")]
        public string credentialName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "displayName cannot be empty.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "displayName cannot be whitespace or contain control characters.")]
        public string displayName { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "credentialId cannot be only whitespace or contain control characters.")]
        public string? credentialId { get; set; }

        //[Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string? credentialUId { get; set; }

        public string? remarks { get; set; }

        public List<long>? categories { get; set; }

        public string? verificationDocType { get; set; }

        [Required]
        [NoNullElements(ErrorMessage = "dataAttributes cannot contain null items.")]
        public List<DataAttributesDTO> dataAttributes { get; set; }

        public string? authenticationScheme { get; set; }

        [StringLength(100)]
        public string? categoryId { get; set; }

        [Range(1, 1000)]
        public int? validity { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "organizationId cannot be empty.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
    ErrorMessage = "organizationId must be a valid GUID.")]
        public string? organizationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(1, ErrorMessage = "trustUrl cannot be empty.")]
        [Url(ErrorMessage = "trustUrl must be a valid URL.")]
        public string trustUrl { get; set; }

        [NoNullElements(ErrorMessage = "serviceDetails cannot contain null items.")]
        public List<string>? serviceDetails { get; set; }

        public string? credentialOffer { get; set; }

        [BindNever]
        public DateTime? createdDate { get; set; }

        [BindNever]
        public DateTime? updatedDate { get; set; }

        public string? signedDocument { get; set; }

        public string? logo { get; set; }

        public string? status { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class DataAttributesDTO
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "displayName cannot be empty.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "displayName cannot be whitespace or contain control characters.")]
        public string displayName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "attribute cannot be empty.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "attribute cannot be whitespace or contain control characters.")]
        public string attribute { get; set; }

        [Range(1, 10, ErrorMessage = "Invalid data type.")]
        public int? dataType { get; set; }
    }
}