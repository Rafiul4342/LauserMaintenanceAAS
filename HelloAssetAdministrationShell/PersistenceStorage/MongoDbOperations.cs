using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.Runtime.Internal.Util;
using BaSyx.AAS.Client.Http;
using BaSyx.API.Components;
using BaSyx.Components.Common;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Components.Common.Abstractions;
using BaSyx.Models.Connectivity;
using BaSyx.Models.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
   
    public class MongoDbOperations
    {
        
        private readonly BasyxStorageAPI<AssetAdministrationShell> _aasStorageApi;
        private readonly BasyxStorageAPI<Submodel> _submodelStorageApi;

        private readonly IAssetAdministrationShellServiceProvider _aasServiceProvider;


        private AssetAdministrationShellHttpClient _client;       
        public MongoDbOperations(
            BasyxStorageAPI<AssetAdministrationShell> aasStorageApi,
            BasyxStorageAPI<Submodel> submodelStorageApi,string Url, IAssetAdministrationShellServiceProvider aasSeviceProvider)
        {   

            _aasStorageApi = aasStorageApi;
            _submodelStorageApi = submodelStorageApi;
           this._client =new AssetAdministrationShellHttpClient(new Uri(Url));
           this._aasServiceProvider = aasSeviceProvider;
        }

       public void UpdateAAS()
        {
            try
            {
                var shell = _client.RetrieveAssetAdministrationShell();
                if (shell.Success == true)
                {
                    var aas =_aasServiceProvider.GetBinding().Cast<AssetAdministrationShell>();
                    Console.WriteLine(aas.GetType());
                    _aasStorageApi.CreatOrUpdateAsync(aas).Wait();
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                    var aas = _client.RetrieveAssetAdministrationShell();
                    if (aas!=null)
                    {
                        Console.WriteLine(aas.ToJson());
                        Submodel data = aas.Entity.Submodels.Value.Cast<Submodel>();
                        Console.WriteLine(data.GetType());
                        _submodelStorageApi.CreatOrUpdateAsync(data).Wait(); 
                    }
                }

            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            
        }

       
        
    }

}