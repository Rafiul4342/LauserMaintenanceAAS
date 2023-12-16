using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAssetAdministrationShell.MqttConnection.TimeMapper
{
    public class SecondsConverter
    {
        public int ConverCurrenthourstosecond(string currentTime)
        {
          string ti = currentTime;

                // Split the time string into hours, minutes, and seconds
                string[] parts = ti.Split(":");

                // Parse hours, ensuring it has at least four digits
                int hh = int.Parse(parts[0].PadLeft(4, '0'));

                // Parse minutes and seconds
                int mm = int.Parse(parts[1]);
                int ss = int.Parse(parts[2]);

                // Convert hours, minutes, and seconds to total seconds
                int totalSeconds = hh * 3600 + mm * 60 + ss;

                return totalSeconds;
                
        }

        public string incrementedtimeformatter(int time)
         {
             int incrementedTime = time + 1;

             // Calculate total hours beyond 24 or 99
             int totalHours = incrementedTime / 3600;

             // Calculate remaining seconds after total hours are determined
             int remainingSeconds = incrementedTime % 3600;

             // Calculate minutes and seconds from remaining seconds
             int mm = remainingSeconds / 60;
             int ss = remainingSeconds % 60;

             return $"{totalHours:D4}:{mm:D2}:{ss:D2}";

         }
    }
}
