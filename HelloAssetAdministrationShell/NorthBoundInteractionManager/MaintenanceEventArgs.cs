using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class MaintenanceEventArgs : EventArgs
    {
        public string Maintenancetype {get;}
        public int ThresoldValue { get; }
        
        public MaintenanceEventArgs(string maintenancetype, int thresoldvalue)
        {
            this.Maintenancetype = maintenancetype;
            this.ThresoldValue = thresoldvalue;
        }
    }
}
