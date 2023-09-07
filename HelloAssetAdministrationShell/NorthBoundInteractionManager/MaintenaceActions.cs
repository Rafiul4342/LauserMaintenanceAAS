using BaSyx.AAS.Client.Http;
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
        public static List<SubmodelElementCollection> InteractionElements = new List<SubmodelElementCollection>();
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
               InteractionElements.Insert(0, submodelElementCollection);
               return InteractionElements;
           }
           else
           {
               Console.WriteLine("Unable to getRecord value");
               return null;
           }
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
            IValue setToZero = new ElementValue("00:00:00");
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
            var InteractionElement = ReceivedData.interactionElements;
            var ConversationID = ReceivedData.frame.conversationId.ToString();
            
            foreach (var VARIABLE in InteractionElement)
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
            }

        }
        public void UpdateMaintenanceHistoryCount(string MaintenanceType)
        {
            var Currentrecord = _client.RetrieveSubmodelElementValue("MaintenanceSubmodel", string.Concat(MaintenanceType, "/", "MaintenanceHistory/MaintenaceCounter"));
            var updateRecord =Convert.ToInt64(Currentrecord.Entity.Value) + 1;
            IValue updatedValue = new ElementValue(updateRecord, typeof(int));
            var updatedr = _client.UpdateSubmodelElementValue("MaintenanceSubmodel", string.Concat(MaintenanceType, "/", "MaintenanceHistory/MaintenaceCounter"), updatedValue);
            if (updatedr.Success)
            {
                Console.WriteLine("Record Update Successfully");
            }
            else 
            {
                Console.WriteLine("Unable to update record");
            }
        }

        

    }
}
