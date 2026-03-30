using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class QrCredentialVerifiersRepository : GenericRepository<QrCredentialVerifier, idp_dtplatformContext>, IQrCredentialVerifiersRepository

    {
        private readonly ILogger _logger;

        public QrCredentialVerifiersRepository(idp_dtplatformContext context,
            ILogger logger) : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<bool> IsCredentialAlreadyExists(QrCredentialVerifierDTO qrCredentialVerifierDTO)
        {
            try
            {
                return await Context.QrCredentialVerifiers.AsNoTracking().AnyAsync(u => u.OrganizationId == qrCredentialVerifierDTO.organizationId && u.CredentialId == qrCredentialVerifierDTO.credentialId);
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
                return await Context.QrCredentialVerifiers
                        .OrderByDescending(u => u.Id)
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

        public async Task<IEnumerable<QrCredentialVerifier>> GetCredentialListDataByOrganizationIdAsync(string organizationId)
        {
            try
            {
                return await Context.QrCredentialVerifiers
                        .OrderByDescending(u => u.Id)
                        .Where(u => u.OrganizationId == organizationId)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetCredentialListDataByOrganizationIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<QrCredentialVerifier>> GetActiveCredentialVerifierListAsync()
        {
            try
            {
                return await Context.QrCredentialVerifiers
                        .Include(u => u.Credential)
                        .OrderByDescending(u => u.Id)
                        .Where(u => u.Status == "SUBSCRIBED")
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveCredentialVerifierListAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<QrCredentialVerifier>> GetActiveCredentialListByOrganizationIdAsync(string orgId)
        {
            try
            {
                return await Context.QrCredentialVerifiers
                        .OrderByDescending(u => u.Id)
                        .Where(u => u.Status == "SUBSCRIBED" && u.OrganizationId == orgId)
                        .ToListAsync();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetActiveCredentialListByOrganizationIdAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<QrCredentialVerifier>> GetCredentialVerifierListByIssuerIdAsync(string organizationId)
        {
            try
            {
                return await Context.QrCredentialVerifiers
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