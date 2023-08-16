using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using HelloAssetAdministrationShell.I40MessageExtension.MessageFormat;
using HelloAssetAdministrationShell.MqttConnection;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class SendMaintenanceOrders
    {
        public string ClinetID;
        public string brokeraddress;
        public int brokerport;
        public string url; 

        public static string messageId;

        public static string ConvessationID;

        public  static string sender = "Machine_AAS_1";

        public static  Dictionary<string, int> myMaintenceCounter = new Dictionary<string, int>();

        public static Dictionary<string, ConversationInfo> ConversationTracker = new Dictionary<string, ConversationInfo>();

        

       public HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient;
       [Obsolete]
        public async Task SendMaintenanceOrders1(string clinetID,string BrokerAddress, int port, string AASurl)
        {
            MaintenceMonitor.MaintenanceEvent += HandleMaintenceOrder;
            this.ClinetID = clinetID;
            this.brokeraddress = BrokerAddress;
            this.brokerport = port;
            this.url = AASurl;

            HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound("test.mosquitto.org", 1883, ClinetID);


            await mqttclient.SubscribeAsync("rafiul");
            mqttclient.MessageReceived += OnMessage;

        }

        public void HandleMaintenceOrder(object sender, MaintenanceEventArgs e)
        {
            Console.WriteLine($"MainteneceIntercal : {e.Maintenancetype} , Maintencethereold : {e.ThresoldValue}");
            
            try
            {

                if (myMaintenceCounter.ContainsKey(e.Maintenancetype))
                {
                    int count = myMaintenceCounter[e.Maintenancetype];
                    int updatedCount = count + 1;
                    myMaintenceCounter[e.Maintenancetype] = updatedCount;
                    
                }
                else if(!myMaintenceCounter.ContainsKey(e.Maintenancetype)){
                    myMaintenceCounter.Add(e.Maintenancetype, 1);
                };
                I40Message message = new I40Message();
                var interactionElement = RetreiveInteractionElement.GetInteractionElement(url,e.Maintenancetype);
                message.interactionElements = interactionElement;
                string ConversationID = e.Maintenancetype + myMaintenceCounter[e.Maintenancetype].ToString(); 
                var frame = CreateFrame.GetFrame(e.Maintenancetype + myMaintenceCounter[e.Maintenancetype].ToString(), "1", "NOTIFY_INIT");
                message.Setframe(frame);
                ConversationTracker.Add(ConversationID, value: new ConversationInfo { MaintenceType = e.Maintenancetype, OrderStatus = "MaintenanceOrderSent", StartTime =DateTime.Now });
                var result = mqttclient.PublishAsync("test", message);              
            }
            catch
            {
                Console.WriteLine("Unable to retreive value");
            }
          
          
        }

        [Obsolete]
        private void OnMessage(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            // Implement your logic here to handle the received message
            Console.WriteLine($"Received message on topic '{topic}': {payload}");

        }
    }
}
