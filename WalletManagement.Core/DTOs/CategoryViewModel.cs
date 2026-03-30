using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletManagement.Core.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CategoryViewModel
    {
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [RegularExpression(@"^([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})?$",
    ErrorMessage = "categoryUid must be a valid GUID or omitted entirely.")]
        public string? CategoryUid { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 1)]
        public string Description { get; set; }

        [BindNever]
        public DateTime? CreatedDate { get; set; }

        [BindNever]
        public DateTime? ModifiedDate { get; set; }

        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Status { get; set; }
    }

}