using MongoDB.Bson.Serialization.Attributes;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Models
{
    internal sealed class PausedTriggerGroup
    {
        [BsonId]
        public PausedTriggerGroupId Id { get; set; } = default!;
    }
}
