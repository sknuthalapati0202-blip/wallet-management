namespace WalletManagement.Core.Domain.Repositories
{
    public interface IGenericUnitOfWork
    {
        void Save();

        Task SaveAsync();
    }
}
