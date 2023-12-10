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
using BaSyx.Models.Core.Common;
using BaSyx.Models.Extensions;
using HelloAssetAdministrationShell.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
namespace HelloAssetAdministrationShell.PersistenceStorage
{
    public class BasyxStorageAPI<T>: IBasyxStorageAPI<T>
    {
        public  ILogger Logger { get; }
        protected readonly Type type;
        
        protected string Collection_Name;
       
        private readonly IMongoDatabase _database;
        
        private readonly BsonClassMap<T> classMap;
       
        private readonly Dictionary<ModelType,string> Mycollections;
        
       
       
        public BasyxStorageAPI(ILogger logger, IMongoDatabase database)
        {
            Logger = logger;
            _database = database;
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            
            BsonClassMap.RegisterClassMap<Identifier>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Id).SetElementName("_id");
                cm.MapMember(c => c.IdType).SetElementName("idType");
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<AssetAdministrationShell>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Identification.Id).SetElementName("_id");
                // Map other properties as needed
            });

            BsonClassMap.RegisterClassMap<Submodel>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Identification.Id).SetElementName("_id");
                // Map other properties as needed
            });
            
            Mycollections = new Dictionary<ModelType, string>
            {
                { ModelType.AssetAdministrationShell, "AssetAdministrationShellCollection" },
                { ModelType.Submodel, "SubmodelCollection" },
                // Add other mappings as needed
            };

        }
        private string GetCollectionName(ModelType modelType)
        {
            if (Mycollections.TryGetValue(modelType, out var collectionName))
            {
                return collectionName;
            }

            throw new ArgumentException($"No mapping found for ModelType: {modelType}");
        }

        public string GetKey(T obj)
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
            var filter = Builders<T>.Filter.Eq("identification.id", key);
            var data= _database.GetCollection<T>(Collection_Name).Find(filter).FirstOrDefault();
            return data;
        }

        private bool AlreadyExist(string key)
        {
            var filter = Builders<T>.Filter.Eq("Identification.Id", key);
            var count = _database.GetCollection<T>(Collection_Name).Find(filter).CountDocuments();
            return count > 0;

        }

        public IEnumerable<T> RetrieveAll()
        {
            return _database.GetCollection<T>(Collection_Name).Find(_ => true).ToList();
        }
     
        public T Update(T obj, string key)
        {
            var filter = Builders<T>.Filter.Eq("Identification.Id", key);
            var options = new FindOneAndReplaceOptions<T> { ReturnDocument = ReturnDocument.After };
            var collection = _database.GetCollection<T>(Collection_Name);
            var replaced = collection.FindOneAndReplace(filter, obj, options);

            if (replaced == null)
            {
                // Document with the specified key does not exist, you may choose to handle this situation
                // For example, you can throw an exception or log a warning
                Console.WriteLine($"Document with key {key} does not exist.");
                return default;
            }

            return replaced;
        }
 
        
        public T CreatOrUpdate(T obj)
        {
            string key = GetKey(obj);
            if (AlreadyExist(key))
            {
                return Update(obj, key);
            }

            var collection = _database.GetCollection<T>(Collection_Name);
            collection.InsertOne(obj);

            return obj;

        }




        public bool Delete(string key)
        {
            var filter = Builders<T>.Filter.Eq("Identification.Id", key);
            var collection = _database.GetCollection<T>(Collection_Name);

            var result = collection.DeleteOne(filter);

            // Return true if at least one document was deleted
            return result.DeletedCount > 0;
        }

        public void CreateCollectionIfNotExists(string collectionName)
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Contains(collectionName))
            {
                _database.CreateCollection(collectionName);
            }
        }

        public void DeleteCollection()
        {
            throw new System.NotImplementedException();
        }
    }
}
