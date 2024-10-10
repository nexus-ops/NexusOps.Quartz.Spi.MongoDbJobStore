namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal sealed class CalendarId : BaseId
    {
        public CalendarId()
        {
            CalendarName = string.Empty;
        }

        public CalendarId(string calendarName, string instanceName)
        {
            InstanceName = instanceName;
            CalendarName = calendarName;
        }

        public string CalendarName { get; set; }
    }
}
