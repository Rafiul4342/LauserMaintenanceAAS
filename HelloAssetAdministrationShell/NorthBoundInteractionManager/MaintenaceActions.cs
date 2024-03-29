﻿using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using HelloAssetAdministrationShell.I40MessageExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenaceActions
    {
      
        public AssetAdministrationShellHttpClient _client { get; set; }
        public SubmodelElementCollection IValue { get; private set; }
        public static List<SubmodelElementCollection> InteractionElements;
        
       public MaintenaceActions(string url)
        {
            this._client = new AssetAdministrationShellHttpClient(new Uri(url));
        }

       public List<SubmodelElementCollection> GetUpDatedRecord(string MaintenanceType)
       {

           var Record = _client.RetrieveSubmodelElement("MaintenanceSubmodel",
               string.Concat(MaintenanceType, "/", "MaintenanceRecord"));
           if (Record.Success)
           {
               var JsonDataRetreived = Record.Entity.ToJson();
               SubmodelElementCollection submodelElementCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(JsonDataRetreived);
               InteractionElements = new List<SubmodelElementCollection>();
               InteractionElements.Add(submodelElementCollection);
               
               return InteractionElements;
           }
           else
           {
               Console.WriteLine("Unable to getRecord value");
               return null;
           }
       }

       public string GetSenderID()
       {
           var res = _client.RetrieveAssetAdministrationShell();
           return  res.Entity.Identification.Id.ToString();
       }

       public string GetMachineID()
       {
           var data = _client.RetrieveSubmodelElementValue("MaintenanceSubmodel",
               "Maintenance_1" + "/" + "MaintenanceOrderDescription" + "/" + "MaintenanceElement");
           return data.Entity.Value.ToString();
       }                 
       
       
       public void UpdateMaintenanceOrderStatus(string MaintenaceType, string OrderStatus)
        {
            IValue updatedValue = new ElementValue(OrderStatus);

            try
            {
                var resp = _client.UpdateSubmodelElementValue("MaintenanceSubmodel", MaintenaceType + "/" + "MaintenanceOrderStatus" +"/"+ "ActualOrderStatus", updatedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateMaintenanceCounter(string MaintenanceType)
        {
            IValue setToZero = new ElementValue("0000:00:00",typeof(string));
            try
            {
                var resetedcounter = _client.UpdateSubmodelElementValue("MaintenanceSubmodel", string.Concat(MaintenanceType, "/", "MaintenanceDetails", "/", "OperatingHours"), setToZero);
                if (resetedcounter.Success)
                {
                    Console.WriteLine("Maintenance counter Reseted successfully");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void UpdateMaintenceRecord(dynamic ReceivedData)
        {
           
            var ConversationID = ReceivedData.frame.conversationId.ToString();
            Console.WriteLine(ConversationID);
            // no need for this logic here 
            var Ie = ReceivedData.interactionElements;
                
            //  dynamic jsonObject = JsonSerializer.Deserialize<dynamic>(Ie);
            var d = Ie[0];
            // SubmodelElementCollection submodelElementCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(d);
            var I = d.value;

            foreach (var VARIABLE in I)
            {
                var id = VARIABLE["idShort"];
                var value = VARIABLE["value"];
                var type = VARIABLE["valueType"];

                try
                {
                    if (value == null && id == "MaintenanceStaff")
                    {
                        value = "MaintenanceStuffNotAssignedYet";
                        IValue Value = new ElementValue(value);
                        var updatedvalue = _client.UpdateSubmodelElementValue("MaintenanceSubmodel",
                            string.Concat(SendMaintenanceOrders.ConversationTracker[ConversationID].MaintenanceType,
                                "/", "MaintenanceRecord/",
                                id.ToString()),
                            Value);
                        if (updatedvalue.Success)
                        {
                            Console.WriteLine($"Record is updated with idShort :{0} Value : {1}", VARIABLE.idShort,
                                VARIABLE.value);
                        }
                        else
                        {
                            Console.WriteLine("Value is not updated");
                        }
                    }
                    else
                    {
                        IValue Value = new ElementValue(value);
                        var updatedvalue = _client.UpdateSubmodelElementValue("MaintenanceSubmodel",
                            string.Concat(SendMaintenanceOrders.ConversationTracker[ConversationID].MaintenanceType,
                                "/", "MaintenanceRecord/",
                                id.ToString()),
                            Value);
                        if (updatedvalue.Success)
                        {
                            Console.WriteLine($"Record is updated with idShort :{0} Value : {1}", VARIABLE.idShort,
                                VARIABLE.value);
                        }
                        else
                        {
                            Console.WriteLine("Value is not updated");
                        }
                    }
                    
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

/*
            foreach (var VARIABLE in I)
            {
                foreach (var VARIABLE in COLLECTION)
                {
                    
                }
                var id = VARIABLE["idShort"];
                IValue value = new ElementValue(VARIABLE["value"],
                    VARIABLE["valueType"]);
                    try
                    {
                        var updatedvalue = _client.UpdateSubmodelElementValue("MaintenanceSubmodel",
                            string.Concat(SendMaintenanceOrders.ConversationTracker[ConversationID].MaintenanceType,
                                "/", "MaintenanceRecord/",
                                id.ToString()),
                            value);

                        if (updatedvalue.Success)
                        {
                            Console.WriteLine($"Record is updated with idShort :{0} Value : {1}", VARIABLE.idShort,
                                VARIABLE.value);
                        }
                        else
                        {
                            Console.WriteLine("Value is not updated");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }*/
            }
       
            
           /* foreach (var VARIABLE in InteractionElement)
            {
                foreach (var inter_Value in VARIABLE.value)
                {
                    foreach (var elementValue in inter_Value)
                    {
                        Console.WriteLine(elementValue);
                        string key = ((JProperty)elementValue).Name;
                        Console.WriteLine(key);
                        Console.WriteLine(elementValue.Value.value);

                        Console.WriteLine(elementValue.Value.value.GetType());



                        Console.WriteLine(elementValue.Value.idShort);
                        Console.WriteLine(elementValue.Value.valueType);
                        try
                        {
                            IValue updatedValue = new ElementValue(elementValue.Value.value);
                            _client.UpdateSubmodelElementValue("MaintenanceSubmodel",
                                string.Concat(SendMaintenanceOrders.ConversationTracker[ConversationID].MaintenanceType, "MaintenanceRecord/",
                                    elementValue.Value.idShort.ToString()),
                                updatedValue);
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                }
            }*/

        
        public bool UpdateMaintenanceHistoryCount(string MaintenanceType)
        {
            var Currentrecord = _client.RetrieveSubmodelElementValue("MaintenanceSubmodel",
                string.Concat(MaintenanceType, "/", "MaintenanceHistory", "/", "MaintenanceCounter"));
            var updateRecord =Convert.ToInt64(Currentrecord.Entity.Value) + 1;
            IValue updatedValue = new ElementValue(updateRecord, typeof(int));
            var updated = _client.UpdateSubmodelElementValue("MaintenanceSubmodel", string.Concat(MaintenanceType, "/", "MaintenanceHistory","/","MaintenanceCounter"), updatedValue);
            return updated.Success;
        }



    }
}
