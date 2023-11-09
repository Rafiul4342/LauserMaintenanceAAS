using System.Collections.Generic;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
    
    /// <summary>
    /// Provides basic methods for creating reading deleting or updating objects in the persistence storage  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    
    
    public interface IBasyxStorageAPI<T>
    {
       // Creates or updates an object 
       
        public T CreatOrUpdate(T obj);
       // updates a particular object 
       
        public T Update(T obj, string key);
        
        public T Retrieve(string key);

        public IEnumerable<T> RetrieveAll();

        public bool Delete(string key);
        
        public void CreateCollectionIfNotExists(string collectionName);

        public void DeleteCollection();
        
    }
}