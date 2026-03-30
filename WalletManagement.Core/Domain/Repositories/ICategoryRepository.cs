using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<IEnumerable<Category>> ListAllCategoryAsync();
        public Task<bool> IsCategoryExistsWithNameAsync(string name);
        public Task<Category> GetCategoryByIdAsync(int catId);
        public Task<string> GetCatNameByCatUIDAsync(string name);

    }
}
