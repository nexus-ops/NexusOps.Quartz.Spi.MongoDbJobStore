using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Quartz.Simpl;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Models
{
    internal sealed class Calendar
    {
        private static readonly IObjectSerializer _objectSerializer = new DefaultObjectSerializer();

        [BsonId]
        public CalendarId Id { get; set; }
        public byte[] Content { get; set; }

        public Calendar()
        {
            Id = new CalendarId();
            Content = [];
        }

        public Calendar(string calendarName, ICalendar calendar, string instanceName)
        {
            Id = new CalendarId(calendarName, instanceName);
            Content = _objectSerializer.Serialize(calendar);
        }

        public ICalendar? GetCalendar()
        {
            return _objectSerializer.DeSerialize<ICalendar>(Content);
        }
    }
}
