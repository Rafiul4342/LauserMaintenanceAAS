namespace HelloAssetAdministrationShell.Setting
{
    public class MqttSouthBound
    {
        public string BrokerAddress { get; set; }
        public int Port { get; set; }
        public string SubscriptionTopic { get; set; }
    }
}