# NexusOps.Quartz.Spi.MongoDbJobStore

### v3.1.0-custom1
- Update package meta to reference NexusOps.
- Add `NuGet.config`.
- Use `net8.0` everywhere.
- Update packages:
  - MongoDB.Bson 2.4.2 -> 2.29.0
  - Quartz 3.0.4 -> 3.13.0
- Serialize `JobDataMap` and store in DB as JSON string (instead of `byte[]` in `base64`).
- Fix `SchedulerId.Id` to be serialized as string (instead of `ObjectId`) - `[BsonRepresentation(BsonType.String)]`.
- In Quartz > 3.5.0 (at least in 3.7.0) new method was added to `IJobStore` - `Task ResetTriggerFromErrorState(TriggerKey triggerKey, CancellationToken cancellationToken = default(CancellationToken));`.
    - Quartz.Spi.MongoDbJobStore <= `4.0.0-preview.1` misses this method implementation.
    - Added custom implementation.
- Fixed MongoDB query `await Collection.Find(...).Project(trigger => trigger.Id.GetTriggerKey()).ToListAsync()` -> `(await Collection.Find(...).Project(trigger => trigger.Id.GetTriggerKey()).ToListAsync()).Select(x => x.Id.GetTriggerKey()).ToList()`
- Rename `src/Directory.Build.props` -> `Directory.Build.props.ORIGINAL` to ignore its configuration.

