using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.Common;
using HelloAssetAdministrationShell.I40MessageExtension.MessageFormat;
using HelloAssetAdministrationShell.MqttConnection.TimeMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    
    public class MaintenceMonitor
    {
        
        public AssetAdministrationShellHttpClient _client;
        public SecondsConverter seconds;
        public static event EventHandler<MaintenanceEventArgs> MaintenanceEvent;
        public MaintenceMonitor(string url)
        {
           
           this._client = new AssetAdministrationShellHttpClient(new Uri(url));
           this.seconds = new SecondsConverter();
 
        }
      
        public async Task Monitor_values(string MaintenceIntevals, int thresold)
        {
            bool thresoldReached = false;
            
            
        while (true)
            {
                var  data = _client.RetrieveSubmodelElementValue("MaintenanceSubmodel", MaintenceIntevals + "/" + "MaintenanceDetails" + "/" + "OperatingHours");
                Console.WriteLine(data.Entity.Value);
                int actualtime = seconds.ConverCurrenthourstosecond((string)data.Entity.Value);
                Console.WriteLine(actualtime);
                if(!thresoldReached && actualtime > thresold)
                {
                    thresoldReached = true;
                    MaintenanceEvent?.Invoke(this, new MaintenanceEventArgs(MaintenceIntevals, thresold));
                }
                else if(thresoldReached && actualtime <= thresold)
                {
                    thresoldReached = false;
                }
               
            await Task.Delay(1000);

             //   var even = _client.RetrieveSubmodelElement("MaintenanceSubmodel", MaintenceIntevals + "/" + "MaintenanceDetails" + "/" + "OperatingHours");
            }
               
        /*   try
           {
               even.Entity.ValueChanged += (sender, e) => {
            //       Console.WriteLine("Subscribed to event");

                   Entity_ValueChanged(thresold, (ValueChangedArgs)e.Value);

               };
           }


       catch
       {
           Console.WriteLine("Submodel Element doesnot exist");
       }
        */
    }
        /*
        private void Entity_ValueChanged(int interval, ValueChangedArgs e)
        {
            int actualIValue = Convert.ToInt32(e.Value);
            Console.WriteLine($"Value change in Interval {interval} .New value : {actualIValue}");

            if (actualIValue == interval)
            {
                Console.WriteLine("Maintence threosld Reached");
            }

        }
        */
       
        
    }
}
