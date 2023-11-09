
/*
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
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
    public abstract class BasyxStorageAPI<T>: IBasyxStorageAPI<T>
    {
        public  ILogger Logger { get; }
        protected readonly Type type;
        protected string Collection_Name;

        protected BasyxStorageAPI(string collectionName,Type t,ILogger logger)
        {
            Collection_Name = collectionName;
            this.type = t;
            Logger = logger;
        }
        
        public BasyxStorageAPI() : this(null, null)
        {
        }
        public BasyxStorageAPI(Type type, string collectionName)
        {
            this.type = type;
            Collection_Name = collectionName;
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

        public abstract T RawRetrieveData(string key);
        public abstract ICollection<T> RawRetrieveAllData();
        public abstract FileInfo GetFile(string key, string parentKey, Dictionary<string, object> objMap);
        public abstract string WriteFile(string key, string parentKey, Stream fileStream, ISubmodelElement submodelElement);
        public abstract void DeleteFile(Submodel submodel, string idShort);

        public abstract Object GetStorageConnection();

        public T Retrieve(string key)
        {
            T retrieved = RawRetrieveData(key);
            if (retrieved is Submodel _)
            {
                return (T)(object)HandleRetrievedSubmodel((Submodel)(object)retrieved);
            }
            return retrieved;
        }
        private Type GetElementClass(IEnumerable<T> collection)
        {
            return collection.First().GetType();
        }
        protected Submodel HandleRetrievedSubmodel(Submodel retrieved)
        {
            Dictionary<string, Dictionary<string, object>> elementMaps =
                (Dictionary<string, Dictionary<string, object>>)retrieved[bind];
            Dictionary<string, ISubmodelElement> elements = EnforceISubmodelElements(elementMaps);
            retrieved.SubmodelElements = elements;
            return retrieved;
        }

        private Dictionary<string, ISubmodelElement> EnforceISubmodelElements(Dictionary<string, Dictionary<string, object>> submodelElementObjectMap)
        {
            Dictionary<string, ISubmodelElement> elements = new Dictionary<string, ISubmodelElement>();

            foreach (var (idShort, elementMap) in submodelElementObjectMap)
            {
                ISubmodelElement element = 
                elements.Add(idShort, element);
            }
            return elements;
        }

        //we need to extend this method first 
      
        public IEnumerable<T> RetrieveAll()
        {
            ICollection<T> retrieves = RawRetrieveAllData();

            if (retrieves != null && retrieves.Any() && retrieves.First() is Submodel)
            {
                return retrieves.Select(submodel =>
                {
                    if (submodel is Submodel)
                    {
                        return (T)(object)HandleRetrievedSubmodel();
                    }
                    return default(T);
                }).Where(item => item != null).ToList();
            }

            return retrieves;
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
}*/