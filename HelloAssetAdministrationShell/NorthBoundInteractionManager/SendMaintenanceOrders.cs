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
        
      //  Dictionary<string, >


        public HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient;
       [Obsolete]
        public async Task SendMaintenanceOrders1(string clinetID)
        {
            MaintenceMonitor.MaintenanceEvent += HandleMaintenceOrder;
             this.ClinetID = clinetID;

            

            HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound("test.mosquitto.org", 1883, ClinetID);


            await mqttclient.SubscribeAsync("rafiul");
            mqttclient.MessageReceived += OnMessage;

        }

       

        public void HandleMaintenceOrder(object sender, MaintenanceEventArgs e)
        {
            Console.WriteLine($"MainteneceIntercal : {e.Maintenancetype} , Maintencethereold : {e.ThresoldValue}");
            
            //mqttclient.PublishAsync("rafiul",  )
          
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
