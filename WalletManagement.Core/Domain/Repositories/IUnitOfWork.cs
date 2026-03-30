namespace WalletManagement.Core.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IWalletConfigurationRepository WalletConfiguration { get; }
        ICategoryRepository Category { get; }
        ICredentialRepository Credential { get; }
        IConfigurationRepository Configuration { get; }

        IProvisionStatusRepository ProvisionStatus { get; }

        ICredentialVerifiersRepository CredentialVerifiers { get; }
        IQrCredentialRepository QrCredential { get; }

        IQrCredentialVerifiersRepository QrCredentialVerifiers { get; }
        IWalletDomainRepository WalletDomain { get; }

        IWalletPurposeRepository WalletPurpose { get; }

        IWalletConsentRepository WalletConsent { get; }

        Task<int> SaveAsync();

        void DisableDetectChanges();

        void EnableDetectChanges();

        int Save();
    }
}
