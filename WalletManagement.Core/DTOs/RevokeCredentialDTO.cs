namespace WalletManagement.Core.DTOs
{
    public class RevokeCredentialDTO
    {
        public string issuerID { get; set; }
        public string suid { get; set; }
        public string credentialType { get; set; }
    }
}
