using System;
using System.Threading.Tasks;
using PersistentKeyValueStorage.Model;

namespace PersistentKeyValueStorage.Repository
{
  public abstract class Base : IPersistentKeyValueStorage
  {
    protected abstract Task ExecuteCreateOrUpdateAsync(Entry entry);
    protected abstract Task<bool> ExecuteDeleteAsync(string keyName);
    protected abstract Task<string> ExecuteGetValueAsync(string keyName);

    public async Task CreateOrUpdateAsync(Entry entry)
    {
      if(entry == null)
        throw new ArgumentNullException(nameof(entry));
      if(String.IsNullOrEmpty(entry.Key) || String.IsNullOrWhiteSpace(entry.Key))
        throw new ArgumentNullException(nameof(entry.Key));
      if(String.IsNullOrEmpty(entry.Value) || String.IsNullOrWhiteSpace(entry.Value))
        throw new ArgumentNullException(nameof(entry.Value));        
      
      await ExecuteCreateOrUpdateAsync(entry);
    }

    public async Task<bool> DeleteAsync(string keyName)
    {
      if(String.IsNullOrEmpty(keyName) || String.IsNullOrWhiteSpace(keyName))      
        throw new ArgumentNullException(nameof(keyName));
      
      return await ExecuteDeleteAsync(keyName);
    }

    public async Task<string> GetValueAsync(string keyName)
    {
      if(String.IsNullOrEmpty(keyName) || String.IsNullOrWhiteSpace(keyName))      
        throw new ArgumentNullException(nameof(keyName));
      
      return await ExecuteGetValueAsync(keyName);
    }
  }
}