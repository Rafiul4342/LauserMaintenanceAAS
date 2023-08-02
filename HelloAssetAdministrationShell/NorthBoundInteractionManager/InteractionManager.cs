using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class InteractionManager
    {
        private readonly AssetAdministrationShellHttpClient client;


        public AssetAdministrationShellHttpClient Setclient(string url)
        {
            AssetAdministrationShellHttpClient cl = new AssetAdministrationShellHttpClient(new Uri(url));

            return cl;
        }
    }
}
