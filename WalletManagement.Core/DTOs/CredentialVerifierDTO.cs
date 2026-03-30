using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CredentialVerifierDTO
    {
        public long? id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$")]
        public string credentialId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "organizationId cannot be empty.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
    ErrorMessage = "organizationId must be a valid GUID.")]
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
        [NoNullElements]
        public List<DataAttributes> attributes { get; set; }

        [Required]
        [NoNullElements]
        public List<CredentialConfig> configuration { get; set; }

        [Required(ErrorMessage = "emails is required.")]
        [NoNullElements]
        public List<string> emails { get; set; }
        public string? status { get; set; }

        [Required(ErrorMessage = "domainConfig is required.")]
        public DomainConfig domainConfig { get; set; }

        [Range(1, 1000)]
        public int? validity { get; set; }

        [BindNever]
        public DateTime? createdDate { get; set; }

        [BindNever]
        public DateTime? updatedDate { get; set; }

        [StringLength(500)]
        public string? remarks { get; set; }
    }


    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class DataAttributes
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "displayName cannot be empty.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "displayName cannot be whitespace or contain control characters.")]
        public string displayName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "attribute cannot be empty.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "attribute cannot be whitespace or contain control characters.")]
        public string attribute { get; set; }

        [Range(1, 10)]
        public int? dataType { get; set; }

        public bool? mandatory { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CredentialConfig
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "format cannot be empty.")]
        public string format { get; set; }

        [StringLength(100)]
        public string? bindingMethod { get; set; }

        [StringLength(100)]
        public string? supportedMethod { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class DomainConfig
    {
        [RegularExpression(@"^(https?://[^\s/$.?#].[^\s]*)?$",
    ErrorMessage = "domain must be a valid URL or omitted entirely.")]
        public string? domain { get; set; }

        [NoNullElements]
        public List<string>? purposesList { get; set; }
    }
}