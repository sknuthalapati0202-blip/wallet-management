using WalletManagement.Core.Domain.Services.Communication;

namespace WalletManagement.Core.DTOs
{
    public class WalletTransactionRequestDTO
    {
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string clientID { get; set; }
        public string suid { get; set; }
        public string presentationToken { get; set; }
        public List<CredentialDetail> profiles { get; set; }
    }

    public class CallStackObject
    {
        public string presentationToken { get; set; }
        public List<CredentialDetail> profiles { get; set; }
    }
}
