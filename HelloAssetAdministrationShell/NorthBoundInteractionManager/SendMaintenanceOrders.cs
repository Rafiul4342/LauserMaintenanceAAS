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

        public static string senderAAS = "BASYX_MACHINE_AAS_1";

        public static Dictionary<string, int> myMaintenceCounter = new Dictionary<string, int>();

        public static Dictionary<string, ConversationInfo> ConversationTracker = new Dictionary<string, ConversationInfo>();
        public MaintenaceActions actions;

        public IEnumerable<object> IEvalue { get; private set; }

        [Obsolete]
        public async Task RetryPolicy(I40Message mes, string ConID)
        {
            Console.WriteLine("Retrypolicy started");
           
            for (int retry = 0; retry < 5;  retry++)
            {
                await Task.Delay(5000);
                
                if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "OrderRequestOnProcess")
                {
                    Console.WriteLine("MainteneaceOrderAccepted Stoping Retry Policy");
                    break;
                }

                else if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "OrderSubmitted")
                {
                    var result = mqttclient.PublishAsync("test", mes);
                   
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
            this.actions = new MaintenaceActions(url);
            mqttclient.MessageReceived += OnMessage;

            await mqttclient.SubscribeAsync(topic);
        }

        private async Task HandleNotify_accepted(dynamic message)
        {
            var data = message;
            var ConversationID = data.frame.ConversationID;
            Console.WriteLine(data);
            var Ie = data.interactionElements;
            Console.WriteLine(Ie);
            ConversationTracker[ConversationID].OrderStatus = "OrderRequestOnProcess";
            actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderRequestOnProcess");
            actions.UpdateMaintenceRecord(data);
           
        }
        public async Task Handle_Change(dynamic message)
        {
            var data = message;
          
            var ConversationID = data.frame.conversationId.ToString();
            actions.UpdateMaintenanceCounter(ConversationTracker[ConversationID].MaintenanceType);
            ConversationTracker[ConversationID].OrderStatus = "OrderCompleted";
            actions.UpdateMaintenceRecord(data);
            
          
            var Ie = actions.GetUpDatedRecord(ConversationTracker[ConversationID].MaintenanceType);
            
            I40Message mess = new I40Message();
            mess.interactionElements = Ie;
            var frame = CreateFrame.GetFrame(ConvessationID, 4, "PROCESS",senderAAS);
            mess.SetInteractionElement(Ie);
            mess.Setframe(frame);

            await mqttclient.PublishAsync("Test", mess);

            //logic to create I.40 Respond message

            actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderCompleted");
           

          
            ConversationTracker[ConversationID].EndTime = DateTime.Now;


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
                string ConversationID = e.Maintenancetype + "::"+ myMaintenceCounter[e.Maintenancetype].ToString();
                var frame = CreateFrame.GetFrame(ConvessationID, 1, "NOTIFY_INIT",senderAAS);
                message.Setframe(frame);
                ConversationTracker.Add(ConversationID, value: new ConversationInfo { MaintenanceType = e.Maintenancetype, ID= senderAAS, OrderStatus = "OrderSubmitted", StartTime = DateTime.Now });
                var result = mqttclient.PublishAsync("test", message);
                if (result.IsCompleted)
                {
                    actions.UpdateMaintenanceOrderStatus(e.Maintenancetype, "OrderSubmitted");
                }
                await RetryPolicy(message, ConversationID);
                string message1 = JsonConvert.SerializeObject(message);
                Console.WriteLine(message1);  
                
            }
            catch
            {
                Console.WriteLine("Unable to retrieve value");
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
            /*
            try { var messageReceived = JsonConvert.DeserializeObject<I40MessageExtension.I40NewMessageType>(payload);

                Console.WriteLine(messageReceived);
                var interactionElement = (messageReceived.InteractionElements);
                foreach (var ie in interactionElement)
                {
                    foreach (var val in ie.Value)
                    {
                        foreach (var inner_val in val.Values)
                        {

                            Console.WriteLine(inner_val.IdShort + inner_val.Value);
                            Console.WriteLine(inner_val);

                        }

                    }
                }
            }
            
            catch ( Exception ex)
            {
                Console.WriteLine(ex.Message); 
            } 
          

            */
           // Implement your logic here to handle the received message
          //  Console.WriteLine($"Received message on topic '{topic}': {payload}");
            try { var datadeserialize = JsonConvert.DeserializeObject<dynamic>(payload);
                
            //    Console.WriteLine(datadeserialize);
              //  Console.WriteLine(datadeserialize.GetType());
                var Frame = datadeserialize.frame;
               // Console.WriteLine(Frame);
               var Receiver = datadeserialize.frame.receiver.identification.id;
               // Console.WriteLine(Receiver);
                var MessageType = datadeserialize.frame.type;
                var ConversationID = datadeserialize.frame.conversationId.ToString();
                Console.WriteLine(ConversationID);
                var Ie = datadeserialize.interactionElements;
                Console.WriteLine(Ie);
                Console.WriteLine(Ie[0]);
                var d = Ie[0];
                var I = d.value;
                Console.WriteLine(I);
                

                Console.WriteLine(Ie);
                var type = Ie.GetType();
                Console.WriteLine(type);

                if (Receiver != senderAAS)
                {
                    return;
                }
                else if(Receiver == senderAAS && ConversationTracker.ContainsKey(ConversationID))
                {
                    var state = ConversationTracker[ConversationID].OrderStatus;
                    if (MessageType == "NOTIFY_ACCEPTED" && state == "OrderSubmitted")
                    {
                        HandleNotify_accepted(datadeserialize);
                    }
                    else if (MessageType == "NOTIFY_ACCEPTED" && state == "OrderRequestOnProcess")
                    {
                        Console.WriteLine("Following Message Already Processed");
                    }
                    else if (MessageType == "Change" && state == "OrderRequestOnProcess")
                    {

                        actions.UpdateMaintenanceCounter(ConversationTracker[ConversationID]);

                        //logic to create I.40 Respond message
                        
                        actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderCompleted");
                        actions.UpdateMaintenceRecord(datadeserialize);
                        
                        ConversationTracker[ConversationID].OrderStatus = "OrderCompleted";
                        ConversationTracker[ConversationID].EndTime = DateTime.Now;

                    }

                    else if (MessageType == "Change" && state == "OrderCompleted")
                    {
                        Console.WriteLine($"maintenceLifecycle already Compleated with history {0}",ConversationTracker[ConversationID].ToString());
                    }
                   
                }
                   
              
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
