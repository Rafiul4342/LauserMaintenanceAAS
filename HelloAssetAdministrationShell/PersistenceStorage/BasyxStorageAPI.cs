

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Semantics;
using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Identification;
using BaSyx.Models.Core.AssetAdministrationShell.Identification.BaSyx;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Extensions;
using HelloAssetAdministrationShell.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
    public class BasyxStorageAPI<T>: IBasyxStorageAPI<T>
    {
        public  ILogger Logger { get; }
        protected readonly Type type;
        protected string Collection_Name;
        private IConfiguration Configuration;
        private readonly IMongoDatabase _database;

        protected BasyxStorageAPI(string collectionName,Type t,ILogger logger, IConfiguration _configuration)
        {
            Collection_Name = collectionName;
            this.type = t;
            Logger = logger;
            Configuration = _configuration;
            
            
        }
        
        
        protected string GetKey(T obj)
        {
            if (!(obj is Identifiable || obj is ConceptDescription|| obj is IAssetAdministrationShellDescriptor || obj is ISubmodelDescriptor))
            {
                    throw new ArgumentException("Can only extract a key from an object of types " + nameof(IIdentifiable) + " or " + nameof(ConceptDescription) + " or " + nameof(IAssetAdministrationShellDescriptor) + " or " + nameof(ISubmodelDescriptor));
            }
            if (obj is ConceptDescription description)
            {
                return description.Identification.Id;
            }
            if (obj is IAssetAdministrationShellDescriptor assetAdministrationShellDescriptor)
            {
                return assetAdministrationShellDescriptor.Identification.Id;
            }
            if (obj is ISubmodelDescriptor submodelDescriptor)
            {
                return submodelDescriptor.Identification.Id;
            }
            else
            {
                return ((IIdentifiable)obj).Identification.Id;
            }
        }
        

        //we need to extend this method first 

        public T Retrieve(string key)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> RetrieveAll()
        {
            throw new System.NotImplementedException();
        }
     

        public T CreatOrUpdate(T obj)
        {
            throw new System.NotImplementedException();
        }

        public T Update(T obj, string key)
        {
            throw new System.NotImplementedException();
        }

        

      

        public bool Delete(string key)
        {
            throw new System.NotImplementedException();
        }

        public void CreateCollectionIfNotExists(string collectionName)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCollection()
        {
            throw new System.NotImplementedException();
        }
    }
}