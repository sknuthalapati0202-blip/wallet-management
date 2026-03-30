using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.Domain.Services
{
    public interface IOrganizationService
    {
        Task<ServiceResult> GetOrganizationDetailsByUIdAsync(string organizationUid);
        Task<string[]> GetOrganizationNamesAndIdAysnc();
    }
}
