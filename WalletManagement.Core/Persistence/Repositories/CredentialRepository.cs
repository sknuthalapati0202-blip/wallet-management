using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class CredentialRepository : GenericRepository<Credential, idp_dtplatformContext>,
            ICredentialRepository
    {
        private readonly ILogger _logger;
        public CredentialRepository(idp_dtplatformContext context, ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<Credential>> GetCredentialListAsync()
        {
            try
            {
                return await Context.Credentials
                    .OrderByDescending(c => c.Id)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCredentialListAsync::Database exception");
                return null;
            }
        }
        public async Task<IEnumerable<Credential>> GetCredentialListByOrgIdAsync(string orgId)
        {
            try
            {
                return await Context.Credentials
                    .OrderByDescending(c => c.Id)
                    .AsNoTracking()
                    .Where(c => c.OrganizationId == orgId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCredentialListByOrgIdAsync::Database exception");
                return null;
            }
        }
        public async Task<IEnumerable<Credential>> GetActiveCredentialListAsync()
        {
            try
            {
                return await Context.Credentials.AsNoTracking().Where(u => u.Status == "ACTIVE").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetActiveCredentialListAsync::Database exception");
                return null;
            }
        }

        public async Task<Credential> GetCredentialByIdAsync(int Id)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().FirstOrDefaultAsync(u => u.Id == Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCredentialByIdAsync::Database exception");
                return null;
            }
        }

        public async Task<bool> CreateCredential(Credential credential)
        {
            try
            {
                var credential1 = await Context.Credentials.AddAsync(credential);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateCredential::Database exception");
                return false;
            }
        }

        public async Task<bool> UpdateCredential(Credential credential)
        {
            try
            {
                Context.Credentials.Update(credential);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCredential::Database exception");
                return false;
            }
        }

        public async Task<DataAttributesDTO> GetAttributesByCredentialId(string CredentialId)
        {
            try
            {
                var credential = await Context.Credentials.AsNoTracking().FirstOrDefaultAsync(u => u.CredentialUid == CredentialId);
                return JsonConvert.DeserializeObject<DataAttributesDTO>(credential.DataAttributes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAttributesByCredentialId::Database exception");
                return null;
            }
        }

        public async Task<Credential> GetCredentialByUidAsync(string Uid)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().FirstOrDefaultAsync(u => u.CredentialUid == Uid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCredentialByUidAsync::Database exception");
                return null;
            }
        }

        public async Task<bool> IsCredentialExistsAsync(string credentialName)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().AnyAsync(u => u.CredentialName.ToLower() == credentialName.ToLower());
            }
            catch (Exception error)
            {
                _logger.LogError(error, "IsCredentialExistsAsync::Database exception");
                throw;
            }
        }
        public async Task<bool> IsCredentialDisplayExistAsync(string displayName)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().AnyAsync(u => u.DisplayName.ToLower() == displayName.ToLower());
            }
            catch (Exception error)
            {
                _logger.LogError(error, "IsCredentialDisplayExistAsync::Database exception");
                throw;
            }
        }

        public async Task<bool> IsCredentialExistsAsync(string credentialName, string credentialId)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().AnyAsync(u => u.CredentialName.ToLower() == credentialName.ToLower() && credentialId != u.CredentialUid);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "IsCredentialExistsAsync(name,id)::Database exception");
                throw;
            }
        }
        public async Task<bool> IsCredentialDisplayExistAsync(string displayName, string credentialId)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().AnyAsync(u => u.DisplayName.ToLower() == displayName.ToLower() && credentialId != u.CredentialUid);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "IsCredentialDisplayExistAsync(displayname,id)::Database exception");
                throw;
            }
        }

        public async Task<List<string>> GetCredentialNameIdListAsync(string orgId)
        {
            try
            {
                return await Context.Credentials.AsNoTracking().Where(c => c.OrganizationId == orgId && c.Status == "ACTIVE").Select(c => $"{c.DisplayName},{c.CredentialUid}").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCredentialNameIdListAsync::Database exception");
                return null;
            }
        }

        public async Task<List<Credential>> GetVerifiableCredentialList(List<string> credentialIdList)
        {
            try
            {
                return await Context.Credentials
                    .OrderByDescending(c => c.Id)
                    .AsNoTracking()
                    .Where(c => c.Status == "ACTIVE" && !credentialIdList.Contains(c.CredentialUid))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetVerifiableCredentialList::Database exception");
                return null;
            }
        }

    }
}
