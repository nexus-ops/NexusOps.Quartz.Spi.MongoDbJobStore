using System;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using Quartz.Impl;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Models
{
    internal sealed class JobDetail
    {
        [BsonId]
        public JobDetailId Id { get; set; } = default!;
        public string? Description { get; set; }
        public Type JobType { get; set; } = default!;
        public JobDataMap JobDataMap { get; set; } = default!;
        public bool Durable { get; set; }
        public bool PersistJobDataAfterExecution { get; set; }
        public bool ConcurrentExecutionDisallowed { get; set; }
        public bool RequestsRecovery { get; set; }

        public JobDetail()
        {
        }

        public JobDetail(IJobDetail jobDetail, string instanceName)
        {
            Id = new JobDetailId(jobDetail.Key, instanceName);
            Description = jobDetail.Description;
            JobType = jobDetail.JobType;
            JobDataMap = jobDetail.JobDataMap;
            Durable = jobDetail.Durable;
            PersistJobDataAfterExecution = jobDetail.PersistJobDataAfterExecution;
            ConcurrentExecutionDisallowed = jobDetail.ConcurrentExecutionDisallowed;
            RequestsRecovery = jobDetail.RequestsRecovery;
        }

        public IJobDetail GetJobDetail()
        {
            // The missing properties are figured out at runtime from the job type attributes
            var detail = new JobDetailImpl()
            {
                // Key = new JobKey(Id.Name, Id.Group),
                Description = Description,
                JobType = JobType,
                JobDataMap = JobDataMap,
                Durable = Durable,
                RequestsRecovery = RequestsRecovery
            };
            
            // CUSTOM
            // NB: Quartz.Spi.MongoDbJobStore references Quart.Net 3.2, but higher versions made JobDetailImpl.Key setter internal,
            // so use reflection to set it.
            PropertyInfo propertyInfo = detail.GetType().GetProperty(nameof(JobDetailImpl.Key))!;
            propertyInfo.SetValue(detail, new JobKey(Id.Name, Id.Group), null);

            return detail;
        }
    }
}
