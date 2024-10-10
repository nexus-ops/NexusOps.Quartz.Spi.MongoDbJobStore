namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal sealed class FiredTriggerId : BaseId
    {
        public string FiredInstanceId { get; set; } = string.Empty;

        public FiredTriggerId()
        {

        }

        public FiredTriggerId(string firedInstanceId, string instanceName)
        {
            InstanceName = instanceName;
            FiredInstanceId = firedInstanceId;
        }
    }
}
