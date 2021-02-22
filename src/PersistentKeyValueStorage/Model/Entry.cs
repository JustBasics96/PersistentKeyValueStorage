using MongoDB.Bson.Serialization.Attributes;

namespace PersistentKeyValueStorage.Model
{
  public class Entry
  {
    [BsonElement("value")]
    public string Value { get; set; }
    [BsonId]
    public string Key { get; set; }
  }
}