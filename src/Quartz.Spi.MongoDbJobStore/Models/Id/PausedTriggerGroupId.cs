namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal sealed class PausedTriggerGroupId : BaseId
    {
        public string Group { get; set; } = string.Empty;

        public PausedTriggerGroupId()
        {
        }

        public PausedTriggerGroupId(string group, string instanceName)
        {
            InstanceName = instanceName;
            Group = group;
        }
    }
}
