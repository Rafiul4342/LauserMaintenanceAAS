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
       
        public static List<SubmodelElementCollection> InteractionElement ;
      
        public static async Task<List<SubmodelElementCollection>> GetInteractionElement(string url, string MaintencaceType)
        {
            await Task.Delay(100);
           AssetAdministrationShellHttpClient Client = new AssetAdministrationShellHttpClient(new Uri(url));

            var IE = Client.RetrieveSubmodelElement("MaintenanceSubmodel", MaintencaceType + "/" + "MaintenanceOrderDescription");
          var result1Josn = IE.Entity.ToJson();
            Console.WriteLine(result1Josn);

            SubmodelElementCollection submodelElementCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(result1Josn);

            InteractionElement = new List<SubmodelElementCollection>();
            
            InteractionElement.Add(submodelElementCollection);
          
            
            return InteractionElement;
           
        }

    }
}
