namespace WalletManagement.Core.DTOs
{
    public class CredentialVerifiersListDTO
    {
        public string credentialName { get; set; }
        public string displayName { get; set; }
        public string credentialId { get; set; }
        public string organizationId { get; set; }
        public string logo { get; set; }
        public List<DataAttributes> attributes { get; set; }
    }
}
