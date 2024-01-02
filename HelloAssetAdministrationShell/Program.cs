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
using NLog;
using NLog.Web;
using System.Threading.Tasks;
using System.Text;
using System;
using MQTTnet.Client;
using System.Threading;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Newtonsoft.Json;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using System.Collections.Generic;
using System.IO;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using System.Linq;
using HelloAssetAdministrationShell.NorthBoundInteractionManager;
using Microsoft.Extensions.Configuration;
using  HelloAssetAdministrationShell.Setting;

namespace HelloAssetAdministrationShell
{
    class Program
    {
        //Create logger for the application
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static object I40MessageExtension { get; private set; }
        
     //   public static IConfiguration Configuration { get; private set; }
        public static IConfiguration Configuration { get; set; }
        
        public static string url = "http://localhost:5180";
        
        
       /* IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables() // Add this line to load values from environment variables
            .Build();
            */
        

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

         //   var mqtt = Configuration["MQTT_BROKER_ADDRESS"];
           // Console.WriteLine(mqtt);

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
            string ClinetID = Guid.NewGuid().ToString();
         //   string Subscriptiontopic = "aas-notification";
           // string publishTopic = "BasyxMesAASOrderHandling";
            var mqttSouthBoundConfig = new MqttSouthBound()
            {
                BrokerAddress = Environment.GetEnvironmentVariable("MQTTSOUTHBOUND_BROKERADDRESS") ?? Configuration["MqttSouthBound:BrokerAddress"],
                Port = int.Parse(Environment.GetEnvironmentVariable("MQTTSOUTHBOUND_PORT") ?? Configuration["MqttSouthBound:Port"]),
                SubscriptionTopic = Environment.GetEnvironmentVariable("MQTTSOUTHBOUND_SUBSCRIPTIONTOPIC") ?? Configuration["MqttSouthBound:SubscriptionTopic"]
            };
            var mqttNorthBoundConfig = new MqttNorthBound()
            {
                BrokerAddress = Environment.GetEnvironmentVariable("MQTTNORTHBOUND_BROKERADDRESS") ?? Configuration["MqttNorthBound:BrokerAddress"],
                Port = int.Parse(Environment.GetEnvironmentVariable("MQTTNORTHBOUND_PORT") ?? Configuration["MqttNorthBound:Port"]),
                SubscriptionTopic = Environment.GetEnvironmentVariable("MQTTNORTHBOUND_SUBSCRIPTIONTOPIC") ?? Configuration["MqttNorthBound:SubscriptionTopic"],
                PublicationTopic = Environment.GetEnvironmentVariable("MQTTNORTHBOUND_PUBLICATIONTOPIC")?? Configuration["MqttNorthBound:PublicationTopic"]                
            };
            
            Console.WriteLine(mqttSouthBoundConfig.BrokerAddress);
          
            SendMaintenanceOrders order = new SendMaintenanceOrders();
            order.SendMaintenanceOrders1(ClinetID, mqttNorthBoundConfig.BrokerAddress,mqttNorthBoundConfig.Port,url,mqttNorthBoundConfig.SubscriptionTopic,mqttNorthBoundConfig.PublicationTopic);
            MqttClientFunction cl = new MqttClientFunction(mqttSouthBoundConfig.BrokerAddress,mqttSouthBoundConfig.Port,mqttSouthBoundConfig.SubscriptionTopic);
            //  HelloAssetAdministrationShell.I40MessageExtension.MqttWrapper.MqttNorthbound mqttclient = new I40MessageExtension.MqttWrapper.MqttNorthbound("test.mosquitto.org", 1883, ClinetID);


            //            await mqttclient.SubscribeAsync("rafiul");
            //          mqttclient.MessageReceived += OnMessage;


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
            
           

            await server.RunAsync();

        }

        [System.Obsolete]
        public static async Task Main(string[] args)
        {
            logger.Info("Starting HelloAssetAdministrationShell's HTTP server...");
          //  string url = "http://localhost:5180";
            Console.WriteLine("this is a new program");
           
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
     
        /*    // to set up enviornmental variables use the following code
                   Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Allows overriding settings with environment variables
                .Build();
     
            string mqttBrokerAddress = Configuration["MQTT_BROKER_ADDRESS"];
            Console.WriteLine(mqttBrokerAddress);
            //string mqttBrokerPort = configuration["MQTT_BROKER_PORT"];
            
*/
            List<string>ListofMaintenanceInterval = new List<string>();
            Dictionary<String, int> MaintenanceConfiguration = new Dictionary<string, int>();
          
       
            Task interactionManager = Task.Run(async () => {


                List<Task> tasks = new List<Task>();

                Dictionary<string, int> maintenanceConfiguration = await MaintenceConfiguration.RetrieveMaintenanceConfiguration(url);

                MaintenceMonitor monitor = new MaintenceMonitor(url);

                foreach (var kvp in maintenanceConfiguration)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    var task = monitor.Monitor_values(kvp.Key, kvp.Value);
                    tasks.Add(task);
                }

                await Task.Run(async () => 
                    { 
                        while (true)
                            {
                                await Task.WhenAll(tasks);
                                await Task.Delay(TimeSpan.FromSeconds(60));
                            }
                     }
               );
               // await MaintenceEventGenerator.Eventmonitoring(url);
                /*  NorthBoundInteractionManager.InteractionManager manager = new NorthBoundInteractionManager.InteractionManager();
                  await manager.Manager(url);
                  var client = manager.getClient();

                  try
                  {

                      var sub = client.RetrieveSubmodels();
                      var result = sub.Entity.Values;
                      foreach(ISubmodel submodel in result)
                      {
                          if(submodel.IdShort == "MaintenanceSubmodel")
                          {
                             var submodelElementsValues =submodel.SubmodelElements.Values;
                             // ISubmodelElementCollection col = (ISubmodelElementCollection)submodelElementsValues.GetEnumerator();
                              foreach(var s in submodelElementsValues)
                              {
                                  Console.WriteLine(s.IdShort);
                                  ListofMaintenanceInterval.Add(s.IdShort);
                                  Console.WriteLine(s.GetType());
                              }

                              foreach(var a in ListofMaintenanceInterval)
                              {
                                  var MaintenceDetatils = client.RetrieveSubmodelElement("MaintenanceSubmodel", a);
                                 var ResultJson = MaintenceDetatils.Entity.ToJson();

                                  SubmodelElementCollection submodelElementsCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(ResultJson);
                                  foreach(var element in submodelElementsCollection.Value)
                                  {

                                    //  Console.WriteLine(element.IdShort);
                                      if(element.IdShort == "MaintenanceDetails")
                                      {
                                          SubmodelElementCollection submodelElementsCollection1 = JsonConvert.DeserializeObject<SubmodelElementCollection>(element.ToJson());
                                          foreach (var data in submodelElementsCollection1.Value)
                                          {

                                             // Console.WriteLine(data.IdShort);
                                              if(data.IdShort == "MaintenanceThreshold")
                                              {
                                                  IValue val = data.GetValue();

                                                  Console.WriteLine(val.Value);
                                                  MaintenanceConfiguration.Add(a, (int)val.Value);
                                                  Console.WriteLine(MaintenanceConfiguration.Count);

                                              }


                                          }
                                      }


                                  }


                              }
                          }
                      }



                  }
                  catch (Exception)
                  {

                      throw;
                  }

                  //  var cl = manager.getClient();

                  /*
                        try
                           {
                               BaSyx.Models.Core.AssetAdministrationShell.Implementations.Submodel val = await manager.GetSubmodels();
                               if(val != null)
                               {
                                   Console.WriteLine(val.IdShort);
                               }
                               else
                               {
                                   System.Threading.Thread.Sleep(1000);
                                   await manager.Manager(url);
                                   var vale =await manager.GetSubmodels();
                                   Console.WriteLine(vale.ToString());

                               }
                           }
                           catch
                           {
                               System.Threading.Thread.Sleep(1000);
                               await manager.Manager(url);
                               var vale = await manager.GetSubmodels();
                               Console.WriteLine(vale.ToString());
                           }
                                 */

            });
           
            await InitializeAsync();
            await interactionManager;
            // Await the interactionManager task before exiting the application

            Console.WriteLine("this is a new program");


        }
    }
}