using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class CredentialVerifiersRepository : GenericRepository<CredentialVerifier, idp_dtplatformContext>, ICredentialVerifiersRepository

    {
        private readonly ILogger _logger;
        public CredentialVerifiersRepository(idp_dtplatformContext context,
            ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<bool> IsCredentialAlreadyExists(CredentialVerifierDTO credentialVerifierDTO)
        {
            try
            {
                return await Context.CredentialVerifiers.AsNoTracking().AnyAsync(u => u.OrganizationId == credentialVerifierDTO.organizationId && u.CredentialId == credentialVerifierDTO.credentialId);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "IsCredentialAlreadyExists::Database exception");
                return false;
            }
        }

        public async Task<List<string>> GetCredentialsListByOrganizationIdAsync(string organizationId)
        {
            try
            {
                return await Context.CredentialVerifiers
                        .Where(u => u.OrganizationId == organizationId)
                        .Select(u => u.CredentialId)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetCredentialsListByOrganizationIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<CredentialVerifier>> GetCredentialListDataByOrganizationIdAsync(string organizationId)
        {
            try
            {
                return await Context.CredentialVerifiers
                        .Where(u => u.OrganizationId == organizationId)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetCredentialListDataByOrganizationIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<CredentialVerifier>> GetActiveCredentialVerifierListAsync()
        {
            try
            {
                return await Context.CredentialVerifiers
                        .Where(u => u.Status == "SUBSCRIBED")
                        .Include(u => u.Credential)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveCredentialVerifierListAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<CredentialVerifier>> GetActiveCredentialListByOrganizationIdAsync(string orgId)
        {
            try
            {
                return await Context.CredentialVerifiers
                        .Where(u => u.Status == "SUBSCRIBED" && u.OrganizationId == orgId)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveCredentialListByOrganizationIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<CredentialVerifier>> GetCredentialVerifierListByIssuerIdAsync(string organizationId)
        {
            try
            {
                return await Context.CredentialVerifiers
                    .Include(cv => cv.Credential)
                    .Where(cv => cv.Credential.OrganizationId == organizationId)
                    .ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetCredentialVerifierListByIssuerIdAsync::Database exception");
                return null;
            }
        }
    }
}
