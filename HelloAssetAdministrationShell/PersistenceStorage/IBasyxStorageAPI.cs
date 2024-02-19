using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace HelloAssetAdministrationShell.PersistenceStorage
{
    
    /// <summary>
    /// Provides basic methods for creating reading deleting or updating objects in the persistence storage  
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    
    
    public interface IBasyxStorageAPI<T> where T : class
    {
        Task<T> RetrieveAsync(string key);
        Task<IEnumerable<T>> RetrieveAllAsync();
        Task<T> UpdateAsync(T obj, string key);
        Task<T> CreatOrUpdateAsync(T obj);
        Task<bool> DeleteAsync(string key);
        Task CreateCollectionIfNotExistsAsync(string collectionName);
        Task DeleteCollectionAsync();
    }
    
}
