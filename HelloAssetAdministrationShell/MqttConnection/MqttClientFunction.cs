
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

using CsvHelper;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Net.Http;
using System.Text.Json;
using HelloAssetAdministrationShell.MqttConnection.TimeMapper;
using Microsoft.Extensions.Configuration;

namespace HelloAssetAdministrationShell.MqttConnection
{
    public class MqttClientFunction
    {
        private readonly MqttClient client;

        private readonly HttpClient httpClient;
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Speed { get; set; }
        public string TimeStamp { get; set; }
        private string CurrentStatus { get; set; }

        private readonly StringBuilder csv;

        private readonly SecondsConverter _secondconverter;


        public MqttClientFunction(string brokerAddress, int port,string topicSubscribe)
        {
            
            // create client instance 
            client = new MqttClient(brokerAddress, port, false, null, null, MqttSslProtocols.None);
            // register to message received 
           
            //client.Connect(Guid.NewGuid().ToString(), settings.UserName, settings.Password);
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            // subscribe to the topic "MacnineData/ID-0000" with QoS 2 
            client.Subscribe(new string[] {topicSubscribe}, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            
        }

        static async void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            // Console.WriteLine("Received: " + System.Text.Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);

            var message = System.Text.Encoding.UTF8.GetString(e.Message);
            Console.WriteLine(message.GetType());

            var jsonObject = JsonConvert.DeserializeObject<dynamic>(message);
            Console.WriteLine(jsonObject);
            Console.WriteLine("name : " + jsonObject["name"]);
            Console.WriteLine("status : " + jsonObject["status"]);
            Console.WriteLine("time :" + jsonObject["time"]);
            Console.WriteLine(jsonObject["status"]);

            //            var Temperature = jsonObject["Temperature"].ToString();
            // var temp = jsonObject["Temperature"].ToSting();

            //          var Humidity = jsonObject["Humidity"].ToString();
            //        var Speed = jsonObject["Speed"];
            //      var TimeStamp = jsonObject["Timestamp"];
            int Status = jsonObject["status"];
            Console.WriteLine($"Convetiing status to {0}:", Status);
            var MachineStatus = jsonObject["MachineStatus"];
           // Console.WriteLine("current Temperture is " + Temperature);
            //Console.WriteLine("current Temperture is " + MachineStatus);

            
            
              //  var csv = new StringBuilder();
               // var newLine = string.Format("Temperature : {0}, Humidity : {1}, Speed : {2}, TimeStamp : {3}, MachineStatus : {4}", Temperature, Humidity, Speed, TimeStamp, MachineStatus);
               // csv.AppendLine(newLine);
               // File.WriteAllText("data.csv", csv.ToString());
               // string mes = JsonConvert.SerializeObject(Temperature);
                HttpClient client = new HttpClient();
                SecondsConverter _secondsConverter = new SecondsConverter();

            //var res = await client.PutAsync("http://172.20.32.1:5180/aas/submodels/HelloSubmodel/submodel/submodelElements/Temperature/value", new StringContent(mes,
            //   Encoding.UTF8, "application/json"));
            //Console.WriteLine("Data send to submodel Element");
            if (Status == 2)
                {
                    try
                    {
                        var counterVal_1 = await client.GetAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_1/MaintenanceDetails/OperatingHours/value");
                        if (counterVal_1.IsSuccessStatusCode)
                        {
                            string CurrentCounterValue_1 = await counterVal_1.Content.ReadAsStringAsync();
                            string  CvC_1 = CurrentCounterValue_1.ToString().Trim('\"');
                            // int currentdata = Convert.ToInt32(CurrentCounterValue);
                            Console.WriteLine("The counter value is : {0}", CvC_1);
                            

                        int cv_1 = _secondsConverter.ConverCurrenthourstosecond(CvC_1);
                        string incV_1 = _secondsConverter.incrementedtimeformatter(cv_1);

                            Console.WriteLine(incV_1);
                            string counterupdate_1 = JsonConvert.SerializeObject(incV_1);
                            var update_1 = await client.PutAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_1/MaintenanceDetails/OperatingHours/value", new StringContent(counterupdate_1,
                                   Encoding.UTF8, "application/json"));
                            Console.WriteLine("Counter_1_valueUpdated");
                        } 
                    }
                    catch
                     {
                        Console.WriteLine("Server with MaintenceCounter_1: connot be accesssed");
                    }

                    try
                    {
                        var counterVal_2 = await client.GetAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_2/MaintenanceDetails/OperatingHours/value");

                        if (counterVal_2.IsSuccessStatusCode)
                        {
                            var CurrentCounterValue_2 = await counterVal_2.Content.ReadAsStringAsync();
                        string CvC_2 = CurrentCounterValue_2.ToString().Trim('\"');
                        Console.WriteLine("The counter value is : {0}", CurrentCounterValue_2);
                           
                        int cv_2 = _secondsConverter.ConverCurrenthourstosecond(CvC_2);
                            Console.WriteLine(cv_2);
                        string incCV_2 = _secondsConverter.incrementedtimeformatter(cv_2);
                            string counterupdate_2 = JsonConvert.SerializeObject(incCV_2);

                            var update_2 = await client.PutAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_2/MaintenanceDetails/OperatingHours/value", new StringContent(counterupdate_2,
                                Encoding.UTF8, "application/json"));
                            Console.WriteLine("Counter_2_valueUpdated");
                        }
                    }
                    catch
                    {

                        Console.WriteLine("Server with MaintenceCounter_2: connot be accesssed");
                    }

                    try
                    {
                        var counterVal_3 = await client.GetAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_3/MaintenanceDetails/OperatingHours/value");

                        if (counterVal_3.IsSuccessStatusCode)
                        {
                            var CurrentCounterValue_3 = await counterVal_3.Content.ReadAsStringAsync();
                            Console.WriteLine($"The counter value is : {0}", CurrentCounterValue_3.ToString());
                        string CvC_3 = CurrentCounterValue_3.ToString().Trim('\"');
                        
                            Console.WriteLine(CvC_3);
                        int cv_3 = _secondsConverter.ConverCurrenthourstosecond(CvC_3);

                            Console.WriteLine(cv_3);
                        string incCv_3 = _secondsConverter.incrementedtimeformatter(cv_3);
                            string counterupdate_3 = JsonConvert.SerializeObject(incCv_3);
                            var update = await client.PutAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_3/MaintenanceDetails/OperatingHours/value", new StringContent(counterupdate_3,
                                   Encoding.UTF8, "application/json"));
                            Console.WriteLine("Counter_3_valueUpdated");
                        }

                    }
                    catch
                    {
                        Console.WriteLine("Server with MaintenceCounter_2: connot be accesssed");
                    }
                    try
                    {
                        var counterVal_4 = await client.GetAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_4/MaintenanceDetails/OperatingHours/value");

                        if (counterVal_4.IsSuccessStatusCode)
                        {
                            var CurrentCounterValue_3 = await counterVal_4.Content.ReadAsStringAsync();
                            Console.WriteLine($"The counter value is : {0}", CurrentCounterValue_3.ToString());
                            string CvC_3 = CurrentCounterValue_3.ToString().Trim('\"');
                        
                            Console.WriteLine(CvC_3);
                            int cv_3 = _secondsConverter.ConverCurrenthourstosecond(CvC_3);

                            Console.WriteLine(cv_3);
                            string incCv_3 = _secondsConverter.incrementedtimeformatter(cv_3);
                            string counterupdate_3 = JsonConvert.SerializeObject(incCv_3);
                            var update = await client.PutAsync("http://localhost:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_4/MaintenanceDetails/OperatingHours/value", new StringContent(counterupdate_3,
                                Encoding.UTF8, "application/json"));
                            Console.WriteLine("Counter_3_valueUpdated");
                        }

                    }
                    catch
                    {
                        Console.WriteLine("Server with MaintenceCounter_2: connot be accesssed");
                    }
                }
            else {
                Console.WriteLine(Status);
            }
            
         
        }


        //http://172.25.48.1:5180/aas/submodels/MaintenanceSubmodel/submodel/submodelElements/Maintenance_1/MaintenanceDetails/OperatingHours/value

        //after your loop
    }
}

