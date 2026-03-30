using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Domain.Services.Communication
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class WalletConfigurationResponse
    {
        public List<CredentialFormats> CredentialFormats { get; set; } = new();
        public List<BindingMethods> BindingMethods { get; set; } = new();
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CredentialFormats
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
            ErrorMessage = "Name cannot contain control characters.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
            ErrorMessage = "DisplayName cannot contain control characters.")]
        public string DisplayName { get; set; }

        public bool isSelected { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class BindingMethods
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "Name cannot be whitespace or contain control characters.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1)]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "DisplayName cannot be only whitespace.")]
        public string DisplayName { get; set; }

        [Required]
        [NoNullElements]
        public List<SupportedMethods> SupportedMethods { get; set; }
    }

    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class SupportedMethods
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Name cannot exceed 150 characters.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$", ErrorMessage = "Name cannot be whitespace or contain control characters.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "DisplayName cannot exceed 150 characters.")]
        [RegularExpression(@"^(?!\s*$)[^\x00-\x1F\x7F]+$",
    ErrorMessage = "DisplayName cannot be whitespace or contain control characters.")]
        public string DisplayName { get; set; }

        public bool isSelected { get; set; }
    }
}