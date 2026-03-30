using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Services.Communication
{
    public class CategoryResponse : BaseResponse<Category>
    {
        public CategoryResponse(Category category) : base(category) { }

        public CategoryResponse(string message) : base(message) { }

        public CategoryResponse(Category category, string message) : base(category, message) { }
    }
}
