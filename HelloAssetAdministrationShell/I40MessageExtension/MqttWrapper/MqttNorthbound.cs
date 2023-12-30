using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper
{
    public class MqttNorthbound 
    {

        private  IMqttClient _mqttClient;

        private  MqttClientOptions _options;

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceived;

        public List<string> mymesseges;

        [Obsolete]
        public MqttNorthbound(string brokerIpAddress, int brokerPort, string clientId,string topic)
        {
           
            _options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerIpAddress, brokerPort)
            .WithClientId(clientId)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
            .Build();

            _mqttClient = new MqttFactory().CreateMqttClient();
           
            ConnectAsync(_options).Wait();
            
            _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessages;
        }

        private async Task ConnectAsync(MqttClientOptions _options)
        {
            try
            {
                await _mqttClient.ConnectAsync(_options);
            }
            catch (Exception ex)
            {
                // Handle connection error
                Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            }
        }

        public void SubscribeAsync(string topic)
        {  
            
               _mqttClient.SubscribeAsync(topic);
        }

        [Obsolete]
        private async Task HandleReceivedMessages(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            string topic = eventArgs.ApplicationMessage.Topic;
            string message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);

            MessageReceived?.Invoke(this, eventArgs);
          //  mymesseges.Add(message);
            Console.WriteLine($"Received message on topic: {topic}");
            Console.WriteLine($"Message: {message}");

            // Perform any additional processing or logic here

            await Task.CompletedTask;
        }
        public async Task PublishAsync<I40Message>(string topic, I40Message payload)
        {
            if (_mqttClient.IsConnected)
            {
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(jsonPayload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                    .Build();

                await _mqttClient.PublishAsync(message);
                Console.WriteLine(message);
                Console.WriteLine("MaintenanceCycleCompleted");
            }
            else
            {
                Console.WriteLine("MQTT client is not connected. Attempting to connect and publish...");

                // Assuming _options is already initialized in your class
                if (_options != null)
                {
                    await ConnectAndPublishAsync(_options, topic, payload);
                }
                else
                {
                    Console.WriteLine("MQTT options are not initialized.");
                }
            }
            

        }
        public List<string> getMessages()
        {
            return mymesseges;
        }
        private async Task ConnectAndPublishAsync<M>(MqttClientOptions options, string topic, M payload)
        {
            try
            {
                await _mqttClient.ConnectAsync(options);
                await PublishAsync(topic, payload); // Recursive call to PublishAsync after connecting
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            }
        }
       
    }

}

