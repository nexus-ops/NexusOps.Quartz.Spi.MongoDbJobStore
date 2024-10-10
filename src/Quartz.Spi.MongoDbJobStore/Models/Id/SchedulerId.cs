using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal sealed class SchedulerId : BaseId
    {
        [BsonRepresentation(BsonType.String)] // CUSTOM
        public string Id { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Id}/{InstanceName}";
        }

        public SchedulerId() { }

        public SchedulerId(string id, string instanceName)
        {
            Id = id;
            InstanceName = instanceName;
        }
    }
}
