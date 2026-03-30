using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CategoryEditViewModel
    {
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CategoryUid cannot be empty.")]
        [RegularExpression(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
    ErrorMessage = "CategoryUid must be a valid GUID.")]
        public string? CategoryUid { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(500, MinimumLength = 1)]
        public string Description { get; set; }

        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Status { get; set; }
    }
}