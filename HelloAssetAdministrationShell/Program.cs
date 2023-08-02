/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Distribution License 1.0 which is available at
* https://www.eclipse.org/org/documents/edl-v10.html
*
* 
*******************************************************************************/
using BaSyx.AAS.Server.Http;
using BaSyx.API.Components;
using BaSyx.Common.UI;
using BaSyx.Common.UI.Swagger;
using BaSyx.Discovery.mDNS;
using BaSyx.Utils.Settings.Types;
using HelloAssetAdministrationShell.MqttConnection;
using HelloAssetAdministrationShell.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using HelloAssetAdministrationShell.I40MessageExtension;
using System.Threading.Tasks;
using System.Text;
using System;
using MQTTnet.Client;
using System.Threading;

namespace HelloAssetAdministrationShell
{
    class Program
    {
        //Create logger for the application
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static object I40MessageExtension { get; private set; }

        [Obsolete]
        private static async Task InitializeAsync()
        {
            ServerSettings serverSettings = ServerSettings.LoadSettingsFromFile("ServerSettings.xml");

            //Initialize generic HTTP-REST interface passing previously loaded server configuration
            AssetAdministrationShellHttpServer server = new AssetAdministrationShellHttpServer(serverSettings);

            //Configure the entire application to use your own logger library (here: Nlog)
            server.WebHostBuilder.UseNLog();

            // server.WebHostBuilder.ConfigureServices(Services => { Services.AddSinglton<GetDataService>(); });
            // additional service Registration



            //Instantiate Asset Administration Shell Service
            HelloAssetAdministrationShellService shellService = new HelloAssetAdministrationShellService();

            //    server.WebHostBuilder.ConfigureServices(services => { services.AddHostedService<GetDataService>(); });
            //Dictate Asset Administration Shell service to use provided endpoints from the server configuration
            shellService.UseAutoEndpointRegistration(serverSettings.ServerConfig);
            //  WebHostBuilder webHostBuilder = new WebHostBuilder();


            //Assign Asset Administration Shell Service to the generic HTTP-REST interface
            server.SetServiceProvider(shellService);

            //Add Swagger documentation and UI
            server.AddSwagger(Interface.AssetAdministrationShell);

            //Add BaSyx Web UI
            server.AddBaSyxUI(PageNames.AssetAdministrationShellServer);
            server.AddBaSyxUI("DashBoard");
            string ClinetID = "test01";

            MqttClientFunction cl = new MqttClientFunction();

            HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound("test.mosquitto.org", 1883, ClinetID);


            await mqttclient.SubscribeAsync("rafiul");
            mqttclient.MessageReceived += OnMessage;


            //Action that gets executued when server is fully started 
            server.ApplicationStarted = () =>
            {
                //Use mDNS discovery mechanism in the network. It is used to register at the Registry automatically.
                shellService.StartDiscovery();
            };



            //Action that gets executed when server is shutting down
            server.ApplicationStopping = () =>
            {
                //Stop mDNS discovery thread
                shellService.StopDiscovery();
            };


            server.Run();
        }

        [System.Obsolete]
       public static async Task Main(string[] args)
        {
            logger.Info("Starting HelloAssetAdministrationShell's HTTP server...");
            string url = "http://localhost:5180";
            Console.WriteLine("this is a new program");


           
            Console.WriteLine("this is a new program");
            await InitializeAsync();

            Task interactionManager = Task.Run(async() => {

                NorthBoundInteractionManager.InteractionManager manager = new NorthBoundInteractionManager.InteractionManager();
                await manager.Manager(url);
                var val = manager.GetSubmodel();

            });




        }

        [Obsolete]
        private static void OnMessage(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            // Implement your logic here to handle the received message
            Console.WriteLine($"Received message on topic '{topic}': {payload}");
        }
    }
}