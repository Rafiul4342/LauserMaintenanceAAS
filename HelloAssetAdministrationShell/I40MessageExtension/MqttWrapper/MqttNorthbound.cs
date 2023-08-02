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
        public MqttNorthbound(string brokerIpAddress, int brokerPort, string clientId)
        {
           
            _options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerIpAddress, brokerPort)
            .WithClientId(clientId)
            .Build();

            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessages;
            ConnectAsync(_options).Wait();

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

        public async Task SubscribeAsync(string topic)
        {
           
            
                await _mqttClient.SubscribeAsync(topic);
         

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
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(jsonPayload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
            Console.WriteLine(message);

        }
        public List<string> getMessages()
        {
            return mymesseges;
        }
        public void Dispose()
        {
            _mqttClient.DisconnectAsync().Wait();
            _mqttClient.Dispose();
        }
    }

}

