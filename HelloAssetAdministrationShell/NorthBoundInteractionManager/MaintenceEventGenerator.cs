using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenceEventGenerator
    {
      
        public static async Task Eventmonitoring(string url)
        {
            Dictionary<string, int> maintenanceConfiguration = await MaintenceConfiguration.RetrieveMaintenanceConfiguration(url);
            MaintenceMonitor monitor = new MaintenceMonitor(url);
            foreach (var kvp in maintenanceConfiguration)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
               
               monitor.Monitor_values(kvp.Key, kvp.Value);
            }

        }
    }
}
