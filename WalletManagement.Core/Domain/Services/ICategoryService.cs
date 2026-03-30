using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface ICategoryService
    {
        public Task<CategoryResponse> CreateCategoryAsync(Category categoryDTO);
        public Task<IEnumerable<Category>> ListCategoryAsync();
        public Task<ServiceResult> GetCategoryListAsync();
        public Task<ServiceResult> GetCategorybyIdAsync(int catId);
        public Task<ServiceResult> DeleteCategorybyIdAsync(int catId);
        public Task<CategoryResponse> UpdateCategoryAsync(Category categoryDTO);
        public Task<ServiceResult> GetCategoryNameAndIdListAsync();
        public Task<string> GetCategoryNamebyUIdAsync(string catId);
        public Task<Dictionary<string, string>> GetCategoryNameAndIdPairAsync();
        public Task<Category> GetCategoryAsync(int id);
    }
}
