using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenaceActions
    {
      
        public AssetAdministrationShellHttpClient _client { get; set; }
        public SubmodelElementCollection IValue { get; private set; }

       public MaintenaceActions(string url)
        {
            this._client = new AssetAdministrationShellHttpClient(new Uri(url));
        }
        public void UpdateMaintenanceOrderStatus(string MaintenaceType, string OrderStatus)
        {
            IValue updatedValue = new ElementValue(OrderStatus, typeof(string));

            try
            {
                var resp = _client.UpdateSubmodelElementValue("MaintenanceSubmodel", MaintenaceType + "/" + "MaintenanceOrderStatus" + "ActualOrderStatus", updatedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateMaintenanceCounter(string MaintenanceType)
        {
            IValue setToZero = new ElementValue("00:00:00",typeof(string));
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
        public void UpdateMaintenceRecord(string MaintenaceType, object record)
        {
            Console.WriteLine(record);
            
            Console.WriteLine(MaintenaceType);
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
