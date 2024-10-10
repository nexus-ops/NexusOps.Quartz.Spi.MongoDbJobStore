﻿using Common.Logging;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Custom;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Repositories;

namespace Quartz.Spi.MongoDbJobStore
{
    /// <summary>
    /// Implements a simple distributed lock on top of MongoDB. It is not a reentrant lock so you can't
    /// acquire the lock more than once in the same thread of execution.
    /// </summary>
    internal sealed class LockManager : IDisposable
    {
        private static readonly TimeSpan SleepThreshold = TimeSpan.FromMilliseconds(1000);

        private static readonly ILog Log = LogManager.GetLogger<LockManager>();

        private readonly LockRepository _lockRepository;

        private readonly ConcurrentList<LockInstance> _pendingLocks = [];

        private readonly SemaphoreSlim _pendingLocksSemaphore = new SemaphoreSlim(1);

        private bool _disposed;

        public LockManager(IMongoDatabase database, string instanceName, string collectionPrefix)
        {
            _lockRepository = new LockRepository(database, instanceName, collectionPrefix);
        }

        public void Dispose()
        {
            EnsureObjectNotDisposed();

            _disposed = true;
            var locks = _pendingLocks.ToArray();
            foreach (var _lock in locks)
            {
                _lock.Dispose();
            }
        }

        public async Task<IDisposable> AcquireLock(LockType lockType, string instanceId)
        {
            while (true)
            {
                EnsureObjectNotDisposed();
                await _pendingLocksSemaphore.WaitAsync();
                try
                {
                    if (await _lockRepository.TryAcquireLock(lockType, instanceId).ConfigureAwait(false))
                    {
                        var lockInstance = new LockInstance(this, lockType, instanceId);
                        AddLock(lockInstance);
                        return lockInstance;
                    }
                }
                finally
                {
                    _pendingLocksSemaphore.Release();
                }

                await Task.Delay(SleepThreshold);
            }
        }
        
        private async Task ReleaseLock(LockInstance lockInstance)
        {
            await _pendingLocksSemaphore.WaitAsync();
            try
            {

                _lockRepository.ReleaseLock(lockInstance.LockType, lockInstance.InstanceId)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                LockReleased(lockInstance);
            }
            finally
            {
                _pendingLocksSemaphore.Release();
            }
        }

        private void EnsureObjectNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(LockManager));
            }
        }

        private void AddLock(LockInstance lockInstance)
        {
            if (_pendingLocks.Contains(lockInstance))
            {
                throw new Exception(
                    $"Same lock instance for lock {lockInstance.LockType} on {lockInstance.InstanceId} already exists");
            }
            _pendingLocks.Add(lockInstance);
        }

        private void LockReleased(LockInstance lockInstance)
        {
            if (!_pendingLocks.Remove(lockInstance))
            {
                Log.Warn(
                    $"Unable to remove pending lock {lockInstance.LockType} on {lockInstance.InstanceId}");
            }
        }

        private class LockInstance : IDisposable
        {
            private readonly LockManager _lockManager;

            private bool _disposed;

            public LockInstance(LockManager lockManager, LockType lockType, string instanceId)
            {
                _lockManager = lockManager;
                LockType = lockType;
                InstanceId = instanceId;
            }

            public string InstanceId { get; }

            public LockType LockType { get; }

            public void Dispose()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(LockInstance),
                        $"This lock {LockType} for {InstanceId} has already been disposed");
                }

                _lockManager.ReleaseLock(this).GetAwaiter().GetResult();

                _disposed = true;
            }
        }
    }
}
