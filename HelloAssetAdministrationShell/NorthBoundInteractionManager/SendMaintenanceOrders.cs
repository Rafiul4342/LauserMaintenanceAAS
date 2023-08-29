using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using HelloAssetAdministrationShell.I40MessageExtension.MessageFormat;
using HelloAssetAdministrationShell.MqttConnection;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient;
        public static string messageId;

        public static string ConvessationID;

        public static string sender = "BASYX_MACHINE_AAS_1";

        public static Dictionary<string, int> myMaintenceCounter = new Dictionary<string, int>();

        public static Dictionary<string, ConversationInfo> ConversationTracker = new Dictionary<string, ConversationInfo>();


        
      
        
        [Obsolete]
        public async Task RetryPolicy(I40Message mes, string ConID)
        {
            Console.WriteLine("Retrypolicy started");
            System.Threading.Thread.Sleep(1000);
            for (int retry = 0; retry < 5;  retry++)
            {
                if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "MaintenanceOrderAccepted")
                {
                    Console.WriteLine("MainteneaceOrderAccepted Stoping Retry Policy");
                    break;
                }

                else if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "MaintenanceOrderSent")
                {
                    var result = mqttclient.PublishAsync("test", mes);
                    await Task.Delay(1000);
                }

            }
        }


        [Obsolete]
        public async void SendMaintenanceOrders1(string clinetID, string BrokerAddress, int port, string AASurl, string topic)
        {
            MaintenceMonitor.MaintenanceEvent += HandleMaintenceOrder;
            this.ClinetID = clinetID;
            this.brokeraddress = BrokerAddress;
            this.brokerport = port;
            this.url = AASurl;
            this.mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound("test.mosquitto.org", 1883, ClinetID, topic);

            mqttclient.MessageReceived += OnMessage;

            await mqttclient.SubscribeAsync(topic);
        }

        public async Task HandleNotify_accepted(object message)
        {

        }

        [Obsolete]
        public async void HandleMaintenceOrder(object sender, MaintenanceEventArgs e)
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
                else if (!myMaintenceCounter.ContainsKey(e.Maintenancetype)) {
                    myMaintenceCounter.Add(e.Maintenancetype, 1);
                };
                I40Message message = new I40Message();
                var interactionElement =await RetreiveInteractionElement.GetInteractionElement(url, e.Maintenancetype);
                message.interactionElements = interactionElement;
                string ConversationID = "DMU80eVo1" + e.Maintenancetype + "::"+ myMaintenceCounter[e.Maintenancetype].ToString();
                var frame = CreateFrame.GetFrame(e.Maintenancetype + myMaintenceCounter[e.Maintenancetype].ToString(), 1, "NOTIFY_INIT");
                message.Setframe(frame);
                ConversationTracker.Add(ConversationID, value: new ConversationInfo { MaintenceType = e.Maintenancetype, OrderStatus = "MaintenanceOrderSent", StartTime = DateTime.Now });
                var result = mqttclient.PublishAsync("test", message);
                await RetryPolicy(message, ConversationID);
                string message1 = JsonConvert.SerializeObject(message);
                Console.WriteLine(message1);
               
               
                
            }
            catch
            {
                Console.WriteLine("Unable to retreive value");
            }


        }

        

        [Obsolete]
        private async void OnMessage(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine(payload.GetType());
            
           // JObject jsonResponse = JObject.Parse(payload);
            //List<JObject> mydata = new List<JObject>();
            //mydata.Add(jsonResponse);

            // Implement your logic here to handle the received message
          //  Console.WriteLine($"Received message on topic '{topic}': {payload}");
            try { var datadeserialize = JsonConvert.DeserializeObject<dynamic>(payload);
                
                Console.WriteLine(datadeserialize);
                var Ie = datadeserialize.interactionElements;
                Console.WriteLine(Ie);
                var type = Ie.GetType();
                Console.WriteLine(type);
                var Frame = datadeserialize.frame;
                Console.WriteLine(Frame);
                string Messagetype = Frame.type;
                var Receiver = Frame.receiver.identification.id;
                Console.WriteLine(Receiver);
                Console.WriteLine(Messagetype);


            }
            catch(Exception ex)
            {
                Console.WriteLine("Message couldnot be deserialized" );
                Console.WriteLine(ex.Message);
            }
            
          
            
           await Task.Delay(100);

        }
    }
}
