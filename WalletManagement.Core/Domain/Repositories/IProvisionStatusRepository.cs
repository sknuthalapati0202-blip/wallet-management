using WalletManagement.Core.Domain.Models;

namespace WalletManagement.Core.Domain.Repositories
{
    public interface IProvisionStatusRepository : IGenericRepository<ProvisionStatus>
    {
        public Task<ProvisionStatus> GetProvisionStatus(string suid, string credentialId);
        public Task<ProvisionStatus> GetProvisionStatusByDocumentId(string credentialId, string documentId);
    }
}
