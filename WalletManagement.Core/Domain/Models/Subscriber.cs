#nullable disable

namespace WalletManagement.Core.Domain.Models
{
    public partial class Subscriber
    {
        public string UserId { get; set; }
        public string MailId { get; set; }
        public string MobileNo { get; set; }
        public int StatusId { get; set; }
        public string DeviceToken { get; set; }
    }
}
