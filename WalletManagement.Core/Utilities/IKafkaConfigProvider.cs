namespace WalletManagement.Core.Utilities
{
    public interface IKafkaConfigProvider
    {
        KafkaConfig GetKafkaConfiguration();
    }
}
