
using HelloAssetAdministrationShell.I40MessageExtension.MessageFormat;

using MQTTnet.Client;


using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using HelloAssetAdministrationShell.NorthBoundInteractionManager.I40CommunicationPersistence;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


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

        public string senderAAS;

        public static Dictionary<string, int> myMaintenceCounter;
        
        private string PublishTopic;

        public static Dictionary<string, ConversationInfo> ConversationTracker; 
       
        public MaintenaceActions actions;

      

        [Obsolete]     
        public async Task RetryPolicy(I40Message mes, string ConID,string topic)
        {
            Console.WriteLine("Retrypolicy started");
           
            for (int retry = 0; retry < 5;  retry++)
            {
                await Task.Delay(500000);
                
                if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "OrderRequestOnProcess")
                {
                    Console.WriteLine("MainteneaceOrderAccepted Stoping Retry Policy");
                    break;
                }

                else if (SendMaintenanceOrders.ConversationTracker.ContainsKey(ConID) && SendMaintenanceOrders.ConversationTracker[ConID].OrderStatus == "OrderSubmitted")
                {
                    var result = mqttclient.PublishAsync(topic, mes);
                   
                }

            }
            
        }


        [Obsolete]
        public async void SendMaintenanceOrders1(string clinetID, string BrokerAddress, int port, string AASurl, string topic,string publishTopic)
        {
            
            myMaintenceCounter = InteractionDataStorage.LoadMaintenanceCounter();
            ConversationTracker = InteractionDataStorage.LoadConversationTracker();
            
            MaintenceMonitor.MaintenanceEvent += HandleMaintenceOrder;
            this.PublishTopic = publishTopic;
            this.ClinetID = clinetID;
            this.brokeraddress = BrokerAddress;
            this.brokerport = port;
            this.url = AASurl;
            this.mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound(BrokerAddress, port, ClinetID, topic);
            this.actions = new MaintenaceActions(url);
            mqttclient.MessageReceived += OnMessage;

            mqttclient.SubscribeAsync(topic);
            
        }

        private async Task HandleNotify_accepted(dynamic message)
        {
            var data = message;
            
            var ConversationID = data.frame.conversationId.ToString();
            ConversationTracker[ConversationID].OrderStatus = "OrderRequestOnProcess";
            InteractionDataStorage.SaveConversationTracker(ConversationTracker);
            actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderRequestOnProcess");
            actions.UpdateMaintenceRecord(data);
            try
            {
               
                I40Message mes = new I40Message();
                var Ie = actions.GetUpDatedRecord(ConversationTracker[ConversationID].MaintenanceType.ToString());
                mes.interactionElements = Ie;
                
                var frame = CreateFrame.GetFrame(ConversationID, 3, "RESPOND",senderAAS);
                mes.SetInteractionElement(Ie);
                mes.Setframe(frame);
                await mqttclient.PublishAsync(PublishTopic, mes);
                Console.WriteLine("Respond message -- {0}",mes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        public async Task Handle_Change(dynamic message)
        {
            var data = message;
          
            var ConversationID = data.frame.conversationId.ToString();
            actions.UpdateMaintenanceCounter(ConversationTracker[ConversationID].MaintenanceType);
           
            actions.UpdateMaintenceRecord(data);
            
          
            var Ie = actions.GetUpDatedRecord(ConversationTracker[ConversationID].MaintenanceType);
            
            I40Message mes = new I40Message();
            mes.interactionElements = Ie;
            var frame = CreateFrame.GetFrame(ConversationID, 5, "RESPOND",senderAAS);
            mes.SetInteractionElement(Ie);
            mes.Setframe(frame);
            
            await mqttclient.PublishAsync(PublishTopic, mes);
            actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderCompleted");
            ConversationTracker[ConversationID].EndTime = DateTime.Now;
            ConversationTracker[ConversationID].OrderStatus = "OrderCompleted";
            InteractionDataStorage.SaveConversationTracker(ConversationTracker);
            
            var res =actions.UpdateMaintenanceHistoryCount(ConversationTracker[ConversationID].MaintenanceType);
            if (res == true)
            {
                actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType,"Default");
                
            }
            else
            {
                Console.WriteLine("Failed to return");
            }
            //logic to create I.40 Respond message
        }

        [Obsolete]
        public async void HandleMaintenceOrder(object sender, MaintenanceEventArgs e)
        {
            Console.WriteLine($"MaintenaneceInterval : {e.Maintenancetype} , Maintencethereold : {e.ThresoldValue}");

            try
            {

                if (myMaintenceCounter.ContainsKey(e.Maintenancetype))
                {
                    int count = myMaintenceCounter[e.Maintenancetype];
                    int updatedCount = count + 1;
                    myMaintenceCounter[e.Maintenancetype] = updatedCount;
                    InteractionDataStorage.SaveMaintenanceCounter(myMaintenceCounter);
                }
                else if (!myMaintenceCounter.ContainsKey(e.Maintenancetype))
                {
                    myMaintenceCounter.Add(e.Maintenancetype, 1);
                    InteractionDataStorage.SaveMaintenanceCounter(myMaintenceCounter);
                }


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                I40Message message = new I40Message();
                var interactionElement =await RetreiveInteractionElement.GetInteractionElement(url, e.Maintenancetype);
                message.interactionElements = interactionElement;
                var count = InteractionDataStorage.LoadMaintenanceCounter();
                var acc =count[e.Maintenancetype];
                
                string ConversationID = actions.GetMachineID()+ e.Maintenancetype +"::"+ acc.ToString(); 
                Console.WriteLine(ConversationID);
                this.senderAAS = actions.GetSenderID();
                var frame = CreateFrame.GetFrame(ConversationID, 1, "NOTIFY_INIT", senderAAS);
                message.Setframe(frame);
                ConversationTracker.Add(ConversationID, value: new ConversationInfo { MaintenanceType = e.Maintenancetype, ID= senderAAS, OrderStatus = "OrderSubmitted", StartTime = DateTime.Now });
                InteractionDataStorage.SaveConversationTracker(ConversationTracker);
                var result = mqttclient.PublishAsync(PublishTopic, message);

                actions.UpdateMaintenanceOrderStatus(e.Maintenancetype, "OrderSubmitted");
                string message1 = JsonConvert.SerializeObject(message);
                Console.WriteLine(message1);
                await RetryPolicy(message, ConversationID,PublishTopic);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                
            //  Console.WriteLine(datadeserialize);
              //Console.WriteLine(datadeserialize.GetType());
                var Frame = datadeserialize.frame;
               // Console.WriteLine(Frame);
               var Receiver = datadeserialize.frame.receiver.identification.id;
               // Console.WriteLine(Receiver);
                var MessageType = datadeserialize.frame.type;
                
                var ConversationID = datadeserialize.frame.conversationId.ToString();
                Console.WriteLine(ConversationID);
                // no need for this logic here 
                /*
                var Ie = datadeserialize.interactionElements;
                
                //  dynamic jsonObject = JsonSerializer.Deserialize<dynamic>(Ie);
                var d = Ie[0];
                var I = d.Value;
                try
                {
                    var collection = JsonConvert.DeserializeObject<SubmodelElementCollection>(I);
                    Console.WriteLine(collection);
                    foreach (var VARIABLE in collection)
                    {
                       
                        Console.WriteLine(VARIABLE.IdShort);
                        Console.WriteLine(VARIABLE.Value);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Console.WriteLine(I);
                

                Console.WriteLine(Ie);
                var type = Ie.GetType();
                Console.WriteLine(type);
*/
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
                    else if (MessageType == "CHANGE" && state == "OrderRequestOnProcess")
                    {
                        Handle_Change(datadeserialize);
                    //    actions.UpdateMaintenanceCounter(ConversationTracker[ConversationID].MaintenanceType);

                        //logic to create I.40 Respond message
                        
                     //   actions.UpdateMaintenanceOrderStatus(ConversationTracker[ConversationID].MaintenanceType, "OrderCompleted");
                       // actions.UpdateMaintenceRecord(datadeserialize);
                        
                       // ConversationTracker[ConversationID].OrderStatus = "OrderCompleted";
                       // ConversationTracker[ConversationID].EndTime = DateTime.Now;

                    }

                    else if (MessageType == "CHANGE" && state == "OrderCompleted")
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
