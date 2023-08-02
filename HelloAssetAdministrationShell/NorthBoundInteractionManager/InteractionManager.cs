using BaSyx.AAS.Client.Http;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;




namespace HelloAssetAdministrationShell.NorthBoundInteractionManager
{
    public class InteractionManager
    {
        public AssetAdministrationShellHttpClient _client;
        public string Url;


        public async Task Manager(string url)
        {
            try
            {
                this.Url = url;
                HttpClient cl = new HttpClient();
                var resoponse = await cl.GetAsync(Url);
                if (resoponse.IsSuccessStatusCode)
                {
                    AssetAdministrationShellHttpClient _client = new AssetAdministrationShellHttpClient(new Uri(Url));
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                   await Manager(url);
                }
            }
            catch 
            {
                System.Threading.Thread.Sleep(1000);
                await Manager(url);
            }

        }

        
        public AssetAdministrationShellHttpClient getClient()
        {
            return _client;
        }
    
        public Submodel GetSubmodel()
        {
            if (_client == null)
            {
                AssetAdministrationShellHttpClient _clinet = new AssetAdministrationShellHttpClient(new Uri(Url));
                var submodels = _client.RetrieveSubmodels();
                var v = submodels.Entity.Value.IdShort;
             
                Console.WriteLine(submodels.Entity.Count);
                
                return (Submodel)submodels;
                
            }
            else
            {
                var submodels = _client.RetrieveSubmodels();
                Console.WriteLine(submodels.Entity.Count);
                return (Submodel)submodels;
            }
        }


      


    }
}
