using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Quartz.Simpl;

namespace Quartz.Spi.MongoDbJobStore.Serializers
{
    internal sealed class JobDataMapSerializer : SerializerBase<JobDataMap>
    {
        private readonly DefaultObjectSerializer _objectSerializer = new DefaultObjectSerializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JobDataMap value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            //var base64 = Convert.ToBase64String(_objectSerializer.Serialize(value));
            //context.Writer.WriteString(base64);

            // CUSTOM
            var bytes = _objectSerializer.Serialize(value);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            context.Writer.WriteString(json);
        }

        public override JobDataMap Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null!;
            }

            //var bytes = Convert.FromBase64String(context.Reader.ReadString());
            //return _objectSerializer.DeSerialize<JobDataMap>(bytes);

            // CUSTOM
            var json = context.Reader.ReadString();
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return _objectSerializer.DeSerialize<JobDataMap>(bytes)!;
        }
    }
}
