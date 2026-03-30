using Microsoft.EntityFrameworkCore;

using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class GenericUnitOfWork<TContext> : IGenericUnitOfWork
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public GenericUnitOfWork(TContext context)
        {
            this.Context = context;
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
