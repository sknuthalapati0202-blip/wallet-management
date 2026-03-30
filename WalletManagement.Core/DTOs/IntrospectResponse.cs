namespace WalletManagement.Core.DTOs
{
    public class IntrospectResponse
    {
        public bool active { get; set; }
        public string client_id { get; set; }
        public string username { get; set; }
        public string scope { get; set; }
        public string org_id { get; set; }
    }
}
