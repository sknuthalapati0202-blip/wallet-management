using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class WalletPurposeRepository : GenericRepository<WalletPurpose, idp_dtplatformContext>, IWalletPurposeRepository
    {
        private readonly ILogger _logger;

        public WalletPurposeRepository(idp_dtplatformContext context, ILogger logger) :
            base(context, logger)
        {
            _logger = logger;
        }

        public async Task<WalletPurpose> GetPurposeByNameAsync(string name)
        {
            try
            {
                return await Context.WalletPurposes.AsNoTracking().SingleOrDefaultAsync(u => u.Name.ToLower() == name.ToLower() && u.Status == "ACTIVE");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetPurposeByNameAsync::Database exception");
                return null;
            }
        }

        public async Task<IEnumerable<WalletPurpose>> ListAllPurposeAsync()
        {
            try
            {
                return await Context.WalletPurposes.Where(u => u.Status != "DELETED").OrderByDescending(p => p.CreatedDate).ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "ListAllPurposeAsync::Database exception");
                return null;
            }
        }

        public async Task<WalletPurpose> GetPurposeById(int id)
        {
            try
            {
                return await Context.WalletPurposes.SingleOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetPurposeById::Database exception");
                return null;
            }
        }
    }
}