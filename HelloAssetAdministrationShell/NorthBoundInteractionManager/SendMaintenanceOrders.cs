using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class SendMaintenanceOrders
    {
        public SendMaintenanceOrders()
        {
            MaintenceMonitor.MaintenanceEvent += HandleMaintenceOrder;
        }

        public void HandleMaintenceOrder(object sender, MaintenanceEventArgs e)
        {
            Console.WriteLine($"MainteneceIntercal : {e.Maintenancetype} , Maintencethereold : {e.ThresoldValue}");
        }
    }
}
