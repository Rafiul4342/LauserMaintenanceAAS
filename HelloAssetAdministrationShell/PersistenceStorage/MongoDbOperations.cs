using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.Runtime.Internal.Util;
using BaSyx.API.Components;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Components.Common.Abstractions;
using BaSyx.Models.Connectivity;
using Microsoft.Extensions.DependencyInjection;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
   
    public class MongoDbOperations
    {
        
        private readonly BasyxStorageAPI<AssetAdministrationShell> _aasStorageApi;
        private readonly BasyxStorageAPI<Submodel> _submodelStorageApi;
        private readonly IAssetAdministrationShellServiceProvider _serviceProvider;
       
        public MongoDbOperations(
            BasyxStorageAPI<AssetAdministrationShell> aasStorageApi,
            BasyxStorageAPI<Submodel> submodelStorageApi, AssetAdministrationShell aas, IAssetAdministrationShellServiceProvider assetAdministrationShellServiceProvider)
        {

            _aasStorageApi = aasStorageApi;
            _submodelStorageApi = submodelStorageApi;
            _serviceProvider = assetAdministrationShellServiceProvider;

        }
        
        
        
    }

}