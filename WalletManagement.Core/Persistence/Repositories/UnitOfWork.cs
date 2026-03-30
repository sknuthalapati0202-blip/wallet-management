using Microsoft.Extensions.Logging;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;

namespace WalletManagement.Core.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly idp_dtplatformContext _idpContext;
        private ILogger _logger;
        private IWalletConfigurationRepository _walletConfiguration;
        private ICredentialRepository _credential;
        private ICategoryRepository _category;
        private IConfigurationRepository _configuration;
        private IProvisionStatusRepository _provisionStatus;
        private ICredentialVerifiersRepository _credentialVerifiers;
        private IQrCredentialRepository _qrCredential;
        private IQrCredentialVerifiersRepository _qrCredentialVerifiers;
        private IWalletDomainRepository _walletDomain;
        private IWalletPurposeRepository _walletPurpose;
        private IWalletConsentRepository _walletConsent;
        public UnitOfWork(idp_dtplatformContext idpContext,
            ILogger Logger)
        {
            _idpContext = idpContext;
            _logger = Logger;
        }
        public IWalletConfigurationRepository WalletConfiguration
        {
            get
            {
                return _walletConfiguration ??= new WalletConfigurationRepository(_idpContext, _logger);
            }
        }

        public ICategoryRepository Category
        {
            get { return _category = _category ?? new CategoryRepository(_idpContext, _logger); }
        }

        public ICredentialRepository Credential
        {
            get
            {
                return _credential ??= new CredentialRepository(_idpContext, _logger);
            }
        }

        public IConfigurationRepository Configuration
        {
            get { return _configuration = _configuration ?? new ConfigurationRepository(_idpContext, _logger); }
        }

        public IProvisionStatusRepository ProvisionStatus
        {
            get { return _provisionStatus = _provisionStatus ?? new ProvisionStatusRepository(_idpContext, _logger); }
        }

        public ICredentialVerifiersRepository CredentialVerifiers
        {
            get { return _credentialVerifiers = _credentialVerifiers ?? new CredentialVerifiersRepository(_idpContext, _logger); }
        }

        public IQrCredentialRepository QrCredential
        {
            get { return _qrCredential = _qrCredential ?? new QrCredentialRepository(_idpContext, _logger); }
        }

        public IQrCredentialVerifiersRepository QrCredentialVerifiers
        {
            get { return _qrCredentialVerifiers = _qrCredentialVerifiers ?? new QrCredentialVerifiersRepository(_idpContext, _logger); }
        }

        public IWalletPurposeRepository WalletPurpose
        {
            get { return _walletPurpose = _walletPurpose ?? new WalletPurposeRepository(_idpContext, _logger); }
        }

        public IWalletDomainRepository WalletDomain
        {
            get { return _walletDomain = _walletDomain ?? new WalletDomainRepository(_idpContext, _logger); }
        }

        public IWalletConsentRepository WalletConsent
        {
            get { return _walletConsent = _walletConsent ?? new WalletConsentRepository(_idpContext, _logger); }
        }


        public async Task<int> SaveAsync()
        {
            return await _idpContext.SaveChangesAsync();
        }

        public void DisableDetectChanges()
        {
            _idpContext.ChangeTracker.AutoDetectChangesEnabled = false;
            return;
        }

        public void EnableDetectChanges()
        {
            _idpContext.ChangeTracker.AutoDetectChangesEnabled = true;
            return;
        }
        public int Save()
        {
            return _idpContext.SaveChanges();
        }

    }
}
