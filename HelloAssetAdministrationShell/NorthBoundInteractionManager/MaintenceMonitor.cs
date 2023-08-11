using BaSyx.Models.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenceMonitor
    {
      
        public int Thresold { set; get; }
        public async Task Monitor_values(string url, string MaintenceIntevals, int thresold)
        {
            this.Thresold = thresold;
            try
            {
                NorthBoundInteractionManager.InteractionManager manager = new NorthBoundInteractionManager.InteractionManager();
                await manager.Manager(url);
                var client = manager.getClient();
               

                try
                {
                    var data = client.RetrieveSubmodelElementValue("MaintenanceSubmodel", MaintenceIntevals + "/" + "MaintenanceDetails" + "/" + "OperatingHours");
                    Console.WriteLine(data.Entity.Value);
                    var even = client.RetrieveSubmodelElement("MaintenanceSubmodel", MaintenceIntevals + "/" + "MaintenanceDetails" + "/" + "OperatingHours");
                    even.Entity.ValueChanged += Entity_ValueChanged;

                }
                catch
                {
                    Console.WriteLine("Submodel Element doesnot exist");
                }

            }
            catch
            {
                Console.WriteLine("Clinent could not be instantiated");
            }
        }

        private void Entity_ValueChanged(object sender, ValueChangedArgs e)
        {
            Console.WriteLine(e.Value);
        }

       
        
    }
}
