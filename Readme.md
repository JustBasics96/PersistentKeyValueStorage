# PersistentKeyValueStorage
This is a Libary to manage Key Value Storages and store them in a database.

## Supported Storages
* In Memory | NonPersitent
* Azure Mongo DB / CosmosDB

## How to Use

### Net 5 DI
```C#
  services.AddSingleton<IPersistentKeyValueStorage, Mongo>();
```

### Methods
```c#
    Task CreateOrUpdateAsync(Model.Entry entry);
    Task<string> GetValueAsync(string keyName);
    Task<bool> DeleteAsync(string keyName); 
```

#### CreateOrUpdateAsync
Creates or updates an Entry object. No return value

#### GetValueAsync
Returns the latest value for the specified key. Not found returns ``` String.Empty ```

#### DeleteAsync
Returns true when delete successfull, otherwise it will return false


### History

02-22-2021 Birthday



