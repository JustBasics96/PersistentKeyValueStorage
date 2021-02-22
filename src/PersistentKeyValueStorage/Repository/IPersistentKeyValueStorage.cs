using System.Threading.Tasks;

namespace PersistentKeyValueStorage.Repository
{
  public interface IPersistentKeyValueStorage
  {
    Task CreateOrUpdateAsync(Model.Entry entry);
    Task<string> GetValueAsync(string keyName);
    Task<bool> DeleteAsync(string keyName);
  }
}