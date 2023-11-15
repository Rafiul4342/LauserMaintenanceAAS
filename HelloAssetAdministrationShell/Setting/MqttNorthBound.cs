namespace HelloAssetAdministrationShell.Setting
{
    public class MqttNorthBound
    {
        public string BrokerAddress { get; set; }
        public int Port { get; set; }
        public string SubscriptionTopic { get; set; }
        public string PublicationTopic { get; set; }
    }
}