using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public static class RetreiveInteractionElement
    {
       
        public static List<SubmodelElementCollection> InteractionElement;

        public static List<SubmodelElementCollection> GetInteractionElement(string url, string MaintencaceType)
        {
            AssetAdministrationShellHttpClient Client = new AssetAdministrationShellHttpClient(new Uri(url));

            var IE = Client.RetrieveSubmodelElement("MaintenanceSubmodel", MaintencaceType + "/" + "MaintenanceDetails/" + "MaintenanceOrderDescription");
            var result1Josn = IE.Entity.ToJson();

            SubmodelElementCollection submodelElementCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(result1Josn);
            
           
            InteractionElement[0] = submodelElementCollection;
            
            return InteractionElement;
        }

    }
}
