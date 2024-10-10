
# NexusOps.Quartz.Spi.MongoDbJobStore

This repository is forked from https://github.com/glucaci/mongodb-quartz-net.

Modified by NexusOps to include new features and fixes.

The author of the MongoDBMigrations repo doesn't update it regularly, leading to referencing outdated, deprecated, vulnerable package versions.

They have published `4.0.0-preview.1` version with some fixed for latest Quartz versions, but it haven't been tested to work for us.
https://www.nuget.org/packages/Quartz.Spi.MongoDbJobStore/4.0.0-preview.1

Custom changes:

- Using custom version suffix for the Nugget package to indicate custom modifications to the original version. For instance, `2.2.0` -> `2.2.0-custom2`.
- All previous/original changes are marked with comment `// ORIGINAL`.
- All custom changes are marked with comment `// CUSTOM`.
- See all changes [Release notes](./ReleaseNotes.md)

Check the knowledge base for detailed instructions of package publish process.

Custom changes:

- Using custom version suffix for the Nugget package to indicate custom modifications to the original version. For instance, `2.2.0` -> `2.2.0-custom2`.
- All previous/original changes are marked with comment `// ORIGINAL`.
- All custom changes are marked with comment `// CUSTOM`.



Check the knowledge base for detailed instructions of package publish process.

MongoDB Job Store for Quartz.NET
================================
Thanks to @chrisdrobison for handing over this project.

## Basic Usage

```cs
var properties = new NameValueCollection();
properties[StdSchedulerFactory.PropertySchedulerInstanceName] = instanceName;
properties[StdSchedulerFactory.PropertySchedulerInstanceId] = $"{Environment.MachineName}-{Guid.NewGuid()}";
properties[StdSchedulerFactory.PropertyJobStoreType] = typeof (MongoDbJobStore).AssemblyQualifiedName;
// I treat the database in the connection string as the one you want to connect to
properties[$"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyDataSourceConnectionString}"] = "mongodb://localhost/quartz";
// The prefix is optional
properties[$"{StdSchedulerFactory.PropertyJobStorePrefix}.collectionPrefix"] = "prefix";

var scheduler = new StdSchedulerFactory(properties);
return scheduler.GetScheduler();
```

## Nuget

```
Install-Package Quartz.Spi.MongoDbJobStore
```