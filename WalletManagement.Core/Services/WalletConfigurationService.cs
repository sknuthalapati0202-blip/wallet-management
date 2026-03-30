using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Domain.Services.Communication;
using WalletManagement.Core.DTOs;

namespace WalletManagement.Core.Services
{
    public class WalletConfigurationService : IWalletConfigurationService
    {
        private readonly ILogger<WalletConfigurationService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public WalletConfigurationService(ILogger<WalletConfigurationService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult> GetWalletConfiguration()
        {
            var list = await _unitOfWork.WalletConfiguration.GetWalletConfigurationList();

            if (list == null)
            {
                return new ServiceResult(false, "Failed to get Wallet Configuration");
            }

            var walletConfigurationDTO = new WalletConfigurationDTO();
            walletConfigurationDTO.dataBindings = new List<DataBindingDTO>();
            walletConfigurationDTO.credentialFormats = new List<string>();

            foreach (var item in list)
            {
                if (item.Name == "BindingMethods")
                {
                    var bindingMethods = JsonConvert.DeserializeObject<List<BindingMethods>>(item.Value);

                    foreach (var bindingMethod in bindingMethods ?? new List<BindingMethods>())
                    {
                        List<string> supportedMethods = new List<string>();

                        foreach (var supportedMethod in bindingMethod.SupportedMethods ?? new List<SupportedMethods>())
                        {
                            if (supportedMethod.isSelected)
                            {
                                supportedMethods.Add(supportedMethod.Name);
                            }
                        }

                        DataBindingDTO dataBindingDTO = new DataBindingDTO()
                        {
                            Name = bindingMethod.Name,
                            SupportedMethods = supportedMethods
                        };

                        walletConfigurationDTO.dataBindings.Add(dataBindingDTO);
                    }
                }

                if (item.Name == "Credentials_Formats")
                {
                    var credentialFormats = JsonConvert.DeserializeObject<List<CredentialFormats>>(item.Value);

                    foreach (var credentialFormat in credentialFormats ?? new List<CredentialFormats>())
                    {
                        if (credentialFormat.isSelected)
                        {
                            walletConfigurationDTO.credentialFormats.Add(credentialFormat.Name);
                        }
                    }
                }
            }

            return new ServiceResult(true, "Successfully got Wallet Configuration", walletConfigurationDTO);
        }

        public async Task<ServiceResult> UpdateWalletConfiguration(WalletConfigurationResponse walletConfigurationResponse)
        {
            try
            {
                var walletConfigurationList = await _unitOfWork.WalletConfiguration.GetWalletConfigurationList();
                foreach (var item in walletConfigurationList)
                {
                    if (item.Name == "BindingMethods")
                    {
                        item.Value = JsonConvert.SerializeObject(walletConfigurationResponse.BindingMethods);

                        _unitOfWork.WalletConfiguration.Update(item);
                        await _unitOfWork.SaveAsync();

                    }
                    if (item.Name == "Credentials_Formats")
                    {
                        item.Value = JsonConvert.SerializeObject(walletConfigurationResponse.CredentialFormats);
                        //item.Value = JsonConvert.SerializeObject(walletConfigurationDTO.credentialFormats);

                        _unitOfWork.WalletConfiguration.Update(item);
                        await _unitOfWork.SaveAsync();
                    }
                }

                return new ServiceResult(true, "Wallet Configuration Updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update Wallet : " + ex.Message);
                return new ServiceResult(false, "Failed to update Wallet Configuration");
            }
        }

        public async Task<ServiceResult> GetConfiguration()
        {
            var list = await _unitOfWork.WalletConfiguration.GetWalletConfigurationList();
            if (list == null)
            {
                return new ServiceResult(false, "Faled to get Wallet Configuration");
            }
            var walletConfigurationResponse = new WalletConfigurationResponse();
            foreach (var item in list)
            {
                if (item.Name == "Credentials_Formats")
                {
                    walletConfigurationResponse.CredentialFormats = JsonConvert.DeserializeObject<List<CredentialFormats>>(item.Value);
                }
                if (item.Name == "BindingMethods")
                {
                    walletConfigurationResponse.BindingMethods = JsonConvert.DeserializeObject<List<BindingMethods>>(item.Value);
                }
            }
            return new ServiceResult(true, "Successfully got Wallet Configuration", walletConfigurationResponse);
        }

        public async Task<ServiceResult> GetWalletConfigurationDetails()
        {
            var list = await _unitOfWork.WalletConfiguration.GetWalletConfigurationList();
            if (list == null)
            {
                return new ServiceResult(false, "Faled to get Wallet Configuration");
            }
            List<WalletConfigurationsDTO> walletConfigurationList = new List<WalletConfigurationsDTO>();

            List<CredentialFormats> credentialFormats = new List<CredentialFormats>();

            List<BindingMethods> bindingMethods = new List<BindingMethods>();

            foreach (var item in list)
            {
                if (item.Name == "BindingMethods")
                {
                    bindingMethods = JsonConvert.DeserializeObject<List<BindingMethods>>(item.Value);
                }
                if (item.Name == "Credentials_Formats")
                {
                    credentialFormats = JsonConvert.DeserializeObject<List<CredentialFormats>>(item.Value);
                }
            }
            foreach (var item in credentialFormats)
            {
                if (item.Name == "vc+json-Id")
                {
                    List<BindingMethodsDTO> bindingMethodsDTO = new List<BindingMethodsDTO>();

                    foreach (var bindingMethod in bindingMethods)
                    {
                        List<SupportedMethodsDTO> supportedMethods = new List<SupportedMethodsDTO>();
                        if (bindingMethod.Name == "DID")
                        {
                            foreach (var supportedMethod in bindingMethod.SupportedMethods)
                            {
                                supportedMethods.Add(new SupportedMethodsDTO()
                                {
                                    Name = supportedMethod.Name,
                                    DisplayName = supportedMethod.DisplayName,
                                });
                            }
                            bindingMethodsDTO.Add(new BindingMethodsDTO()
                            {
                                Name = bindingMethod.Name,
                                DisplayName = bindingMethod.DisplayName,
                                supportedMethods = supportedMethods
                            });
                            walletConfigurationList.Add(new WalletConfigurationsDTO()
                            {
                                Name = item.Name,
                                DisplayName = item.DisplayName,
                                bindingMethods = bindingMethodsDTO
                            });
                        }
                    }
                }
                if (item.Name == "mso_mdoc")
                {
                    List<BindingMethodsDTO> bindingMethodsDTO = new List<BindingMethodsDTO>();

                    foreach (var bindingMethod in bindingMethods)
                    {
                        List<SupportedMethodsDTO> supportedMethods = new List<SupportedMethodsDTO>();
                        if (bindingMethod.Name == "Cosekey")
                        {
                            if (bindingMethod.SupportedMethods != null)
                            {
                                foreach (var supportedMethod in bindingMethod.SupportedMethods)
                                {
                                    supportedMethods.Add(new SupportedMethodsDTO()
                                    {
                                        Name = supportedMethod.Name,
                                        DisplayName = supportedMethod.DisplayName,
                                    });
                                }
                            }
                            bindingMethodsDTO.Add(new BindingMethodsDTO()
                            {
                                Name = bindingMethod.Name,
                                DisplayName = bindingMethod.DisplayName,
                                supportedMethods = supportedMethods
                            });
                            walletConfigurationList.Add(new WalletConfigurationsDTO()
                            {
                                Name = item.Name,
                                DisplayName = item.DisplayName,
                                bindingMethods = bindingMethodsDTO
                            });
                        }
                    }
                }
            }
            return new ServiceResult(true, "Successfully got Wallet Configuration", walletConfigurationList);
        }
    }
}
