using System;
using System.Threading.Tasks;
using BaSyx.AAS.Client.Http;
using BaSyx.API.Components;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace HelloAssetAdministrationShell.PersistenceStorage.ControllerGETandPOST
{
    [Route("agent/I40Com")]
    [ApiController]
    public class DatabaseControllerClass : Controller
    {

        private readonly IBasyxStorageAPI<AssetAdministrationShell> _basyxStorage;
        private readonly AssetAdministrationShellHttpClient _client;

        DatabaseControllerClass(IBasyxStorageAPI<AssetAdministrationShell> basyxStorage)
        {
            this._basyxStorage = basyxStorage;
            this._client = new AssetAdministrationShellHttpClient(new Uri("http://localhost:5180"));
        }
        [HttpPost]
        public async Task Sendgetdata()
        {
           
            
        }

        [HttpGet]
        public async Task GetAAS()
        {
            
        }
        
    }
}