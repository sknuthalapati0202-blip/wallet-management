using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface ISelfServiceConfigurationService
    {
        Task<ServiceResult> GetAllConfigCategories();
        Task<ServiceResult> GetCategoryFieldNameById(int id);
        Task<ServiceResult> GetCategoryByOrganizationId(string organizationId);
    }
}
