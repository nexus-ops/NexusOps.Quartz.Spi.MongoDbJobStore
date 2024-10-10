﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Models
{
    internal enum SchedulerState
    {
        Started,
        Running,
        Paused,
        Resumed
    }

    internal sealed class Scheduler
    {
        [BsonId]
        public SchedulerId Id { get; set; } = default!;

        [BsonRepresentation(BsonType.String)]
        public SchedulerState State { get; set; }

        public DateTime? LastCheckIn { get; set; }
    }
}
