using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
    /* public class BasyxStorageAPI<T>: IBasyxStorageAPI<T>
     {
        
         protected readonly Type type;
         
         protected string Collection_Name;
        
         private readonly IMongoDatabase _database;
         
         private readonly BsonClassMap<T> classMap;
        
         private readonly Dictionary<ModelType,string> Mycollections;
         
        
        
         public BasyxStorageAPI(IMongoDatabase database)
         {
            
             _database = database;
             var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
             ConventionRegistry.Register("camelCase", conventionPack, t => true);
    
             //RegisterClassMap<AssetAdministrationShell>();
             //RegisterClassMap<Submodel>();
             //RegisterClassMap<Identifier>();
             
           
           
             
             Mycollections = new Dictionary<ModelType, string>
             {
                 { ModelType.AssetAdministrationShell, "AssetAdministrationShellCollection" },
                 { ModelType.Submodel, "SubmodelCollection" },
                 // Add other mappings as needed
             };
 
         }
 
      /*   private void RegisterClassMap<TClass>() where TClass : class
         {
             BsonClassMap.RegisterClassMap<TClass>(cm =>
             {
                 cm.AutoMap();
                 cm.SetIsRootClass(true);
 
                 cm.MapIdMember(c => ((IIdentifiable)c)?.Identification?.Id).SetElementName("_id");
 
                 if (typeof(IIdentifiable).IsAssignableFrom(typeof(TClass)))
                 {
                     cm.MapMember(c => ((IIdentifiable)c)?.Identification?.Id);
                 }
             });
         }
 
         private string GetCollectionName(ModelType modelType)
         {
             if (Mycollections.TryGetValue(modelType, out var collectionName))
             {
                 return collectionName;
             }
 
             throw new ArgumentException($"No mapping found for ModelType: {modelType}");
         }
 
      /*   public string GetKey(T obj)
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
      
         public T   Update(T obj, string key)
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
     }*/

    public class BasyxStorageAPI<T> : IBasyxStorageAPI<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDatabase _database;
        public BasyxStorageAPI(IMongoDatabase database)
        {
            
            string collectionName = typeof(T).Name;
            CreateCollectionIfNotExistsAsync(collectionName).Wait(); // Wait for the collection creation to finish
            this._database = database;
            _collection = _database.GetCollection<T>(GetCollectionName());
        }

       private string GetCollectionName()
    {
        return typeof(T).Name + "Collection";
    }

    private string GetKey(T obj)
    {
        if (obj is IIdentifiable identifiable)
        {
            return identifiable.Identification.Id;
        }

        throw new ArgumentException("Can only extract a key from an object implementing " + nameof(IIdentifiable));
    }

    private bool AlreadyExist(string key)
    {
        var filter = Builders<T>.Filter.Eq("Identification.id", key);
        var count = _collection.Find(filter).CountDocuments();
        return count > 0;
    }

    public async Task<T> RetrieveAsync(string key)
    {
        var filter = Builders<T>.Filter.Eq("Identification.id", key);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> RetrieveAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> UpdateAsync(T obj, string key)
    {
        var filter = Builders<T>.Filter.Eq("Identification.id", key);
        var options = new FindOneAndReplaceOptions<T> { ReturnDocument = ReturnDocument.After };
        return await _collection.FindOneAndReplaceAsync(filter, obj, options);
    }

    public async Task<T> CreatOrUpdateAsync(T obj)
    {
        string key = GetKey(obj);

        if (AlreadyExist(key))
        {
            return await UpdateAsync(obj, key);
        }

        var idProperty = typeof(T).GetProperty("Identification");
        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(obj);
            var idPropertyInIdentification = idValue.GetType().GetProperty("id");
            var id = idPropertyInIdentification.GetValue(idValue);

            var bsonId = BsonValue.Create(id);
            var bsonDocument = obj.ToBsonDocument();
            bsonDocument["_id"] = bsonId;

            await _collection.InsertOneAsync(obj);
        }
        else
        {
            // Handle the case where the object doesn't have an Identification property
        }

        return obj;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        var filter = Builders<T>.Filter.Eq("Identification.id", key);
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task CreateCollectionIfNotExistsAsync(string collectionName)
    {
        try
        {
            var collectionNames = await _database.ListCollectionNames().ToListAsync();
            if (!collectionNames.Contains(collectionName))
            {
                await _database.CreateCollectionAsync(collectionName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task DeleteCollectionAsync()
    {
        await _collection.Database.DropCollectionAsync(_collection.CollectionNamespace.CollectionName);
    }

    }
}
