using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersistentKeyValueStorage.Model;

namespace PersistentKeyValueStorage.Repository
{
  public class Memory : Base
  {
    private Dictionary<string, Model.Entry> _memory;
    public Memory()
    {
      _memory = new Dictionary<string, Entry>();
    }
    protected override Task ExecuteCreateOrUpdateAsync(Entry entry)
    {
      if(_memory.ContainsKey(entry.Key))
        _memory[entry.Key].Value = entry.Value;
      else
        _memory.Add(entry.Key, entry);
      return Task.CompletedTask;
    }

    protected override Task<bool> ExecuteDeleteAsync(string keyName)
    {
      if(!_memory.ContainsKey(keyName))
        return Task.FromResult(false);
      
      _memory.Remove(keyName);
      return Task.FromResult(true);
    }

    protected override Task<string> ExecuteGetValueAsync(string keyName)
    {
      _memory.TryGetValue(keyName, out var entry);
      return entry != null ? Task.FromResult(entry.Value) : Task.FromResult(String.Empty);
    }
  }
}