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

        MaintenaceActions(string url)
        {
            this._client = new AssetAdministrationShellHttpClient(new Uri(url));
        }
        public void UpdateMaintenanceOrderStatus(string MaintenaceType, string OrderStatus)
        {
            IValue updatedValue = new ElementValue(OrderStatus, typeof(string));

            _client.UpdateSubmodelElementValue("MaintenanceSubmodel", MaintenaceType + "/" + "MaintenanceOrderStatus" + "ActualOrderStatus", updatedValue);
        }

        public void UpdateMaintenceRecord(string MaintenaceType, SubmodelElementCollection record)
        {
           
            //_client.CreateOrUpdateSubmodelElement()

        }

    }
}
