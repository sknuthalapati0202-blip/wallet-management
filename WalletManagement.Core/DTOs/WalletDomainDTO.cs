namespace WalletManagement.Core.DTOs
{
    public class WalletDomainDTO
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public Dictionary<string, string> purposes { get; set; }
    }
}
