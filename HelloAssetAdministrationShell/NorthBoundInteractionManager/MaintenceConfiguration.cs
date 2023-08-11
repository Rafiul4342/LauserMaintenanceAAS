using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenceConfiguration
    {
        public static async Task<Dictionary<string, int>> RetrieveMaintenanceConfiguration(string url)
        {
            Dictionary<string, int> maintenanceConfiguration = new Dictionary<string, int>();

            try
            {
                NorthBoundInteractionManager.InteractionManager manager = new NorthBoundInteractionManager.InteractionManager();
                await manager.Manager(url);
                var client = manager.getClient();

                var sub = client.RetrieveSubmodels();
                var result = sub.Entity.Values;

                foreach (ISubmodel submodel in result)
                {
                    if (submodel.IdShort == "MaintenanceSubmodel")
                    {
                        var submodelElementsValues = submodel.SubmodelElements.Values;

                        foreach (var s in submodelElementsValues)
                        {
                            Console.WriteLine(s.IdShort);
                            string submodelElementId = s.IdShort;

                            var maintenanceDetails = client.RetrieveSubmodelElement("MaintenanceSubmodel", submodelElementId);
                            var resultJson = maintenanceDetails.Entity.ToJson();

                            SubmodelElementCollection submodelElementsCollection = JsonConvert.DeserializeObject<SubmodelElementCollection>(resultJson);

                            foreach (var element in submodelElementsCollection.Value)
                            {
                                if (element.IdShort == "MaintenanceDetails")
                                {
                                    SubmodelElementCollection submodelElementsCollection1 = JsonConvert.DeserializeObject<SubmodelElementCollection>(element.ToJson());

                                    foreach (var data in submodelElementsCollection1.Value)
                                    {
                                        if (data.IdShort == "MaintenanceThreshold")
                                        {
                                            IValue val = data.GetValue();
                                            maintenanceConfiguration.Add(submodelElementId, (int)val.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return maintenanceConfiguration;


        }

    }
}
