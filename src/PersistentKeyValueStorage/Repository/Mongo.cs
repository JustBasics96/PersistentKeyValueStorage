using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace PersistentKeyValueStorage.Repository
{
  public class Mongo : IPersistentKeyValueStorage
  {
    private const string CONFIG_PREFIX = "Connections:PersistentKeyValueStorage:";    
    public const string CONNECTION_STRING = CONFIG_PREFIX + "ConnectionString";
    public const string DATABASE_NAME = CONFIG_PREFIX + "Database";
    private const string COLLECTION_NAME = "PersistentKeyValueStorage";
    
    private readonly IConfiguration _configuration;

    private readonly MongoClient _client;
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<Model.Entry> _collection;

    public Mongo(IConfiguration configuration)
    {
      this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      this.ValidateConfiguration();
      this._client = new MongoClient(_configuration[CONNECTION_STRING]);
      this._db = _client.GetDatabase(_configuration[DATABASE_NAME]);
      this._collection = _db.GetCollection<Model.Entry>(COLLECTION_NAME);
    }
    public async Task CreateOrUpdateAsync(Model.Entry entry)
    {
       if(await _collection.CountDocumentsAsync(filter => filter.Key == entry.Key) > 0)
       {
         await _collection.UpdateOneAsync(x => x.Key == entry.Key,UpdateValue(entry.Value));
       }else{
         await _collection.InsertOneAsync(entry);
       }
    }

    public async Task<string> GetValueAsync(string keyName)
    {
      var result = await _collection.Find(x => x.Key == keyName).FirstOrDefaultAsync();
      return result != null ? result.Value : String.Empty;
    }

    public async Task<bool> DeleteAsync(string keyName)
    {
      var result = await _collection.DeleteOneAsync<Model.Entry>(x => x.Key == keyName);
      return result.DeletedCount > 0;
    }

    private void ValidateConfiguration()
    {
      if (string.IsNullOrEmpty(_configuration[CONNECTION_STRING]))
        throw new Exceptions.InvalidConfigurationException($"Missing mandatory config {CONNECTION_STRING}");

      if (string.IsNullOrEmpty(_configuration[DATABASE_NAME]))
        throw new Exceptions.InvalidConfigurationException($"Missing mandatory config {DATABASE_NAME}");
    }    

    private UpdateDefinition<Model.Entry> UpdateValue(string value)
      => Builders<Model.Entry>.Update.Set(x => x.Value, value);    
  }
}
