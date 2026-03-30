namespace WalletManagement.Core.DTOs
{
    public class QrCredentialVerifiersListDTO
    {
        public string credentialName { get; set; }
        public string credentialId { get; set; }
        public string organizationId { get; set; }
        public List<QrAttributesDTO> attributes { get; set; }
    }
}
