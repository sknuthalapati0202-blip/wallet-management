namespace WalletManagement.Core.Utilities
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; }
        public string CentralLogTopic { get; set; }
        public string ServiceLogTopic { get; set; }
        public string AdminLogTopic { get; set; }
    }
}
