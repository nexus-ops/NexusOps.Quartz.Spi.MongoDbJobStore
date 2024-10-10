﻿using System;
using System.Threading.Tasks;
using Common.Logging;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    [CollectionName("locks")]
    internal sealed class LockRepository : BaseRepository<Lock>
    {
        private static readonly ILog _log = LogManager.GetLogger<LockRepository>();

        public LockRepository(IMongoDatabase database, string instanceName, string? collectionPrefix = null)
            : base(database, instanceName, collectionPrefix)
        {
        }

        public async Task<bool> TryAcquireLock(LockType lockType, string instanceId)
        {
            var lockId = new LockId(lockType, InstanceName);
            _log.Trace($"Trying to acquire lock {lockId} on {instanceId}");
            try
            {
                await Collection.InsertOneAsync(new Lock
                {
                    Id = lockId,
                    InstanceId = instanceId,
                    AquiredAt = DateTime.Now
                }).ConfigureAwait(false);
                _log.Trace($"Acquired lock {lockId} on {instanceId}");
                return true;
            }
            catch (MongoWriteException)
            {
                _log.Trace($"Failed to acquire lock {lockId} on {instanceId}");
                return false;
            }
        }

        public async Task<bool> ReleaseLock(LockType lockType, string instanceId)
        {
            var lockId = new LockId(lockType, InstanceName);
            _log.Trace($"Releasing lock {lockId} on {instanceId}");
            var result =
                await Collection.DeleteOneAsync(
                    FilterBuilder.Where(@lock => @lock.Id == lockId && @lock.InstanceId == instanceId)).ConfigureAwait(false);
            if (result.DeletedCount > 0)
            {
                _log.Trace($"Released lock {lockId} on {instanceId}");
                return true;
            }
            else
            {
                _log.Warn($"Failed to release lock {lockId} on {instanceId}. You do not own the lock.");
                return false;
            }
        }

        public override async Task EnsureIndex()
        {
            // ORIGINAL
            //await Collection.Indexes.CreateOneAsync(
            //    IndexBuilder.Ascending(@lock => @lock.AquiredAt),
            //    new CreateIndexOptions() { ExpireAfter = TimeSpan.FromSeconds(30)}
            //).ConfigureAwait(false);

            // CUSTOM
            await Collection.Indexes.CreateOneAsync(
                new CreateIndexModel<Lock>(
                    new IndexKeysDefinitionBuilder<Lock>()
                        .Ascending(@lock => @lock.AquiredAt),
                    new CreateIndexOptions() { ExpireAfter = TimeSpan.FromSeconds(30) }
                )
            ).ConfigureAwait(false);
        }
    }
}
