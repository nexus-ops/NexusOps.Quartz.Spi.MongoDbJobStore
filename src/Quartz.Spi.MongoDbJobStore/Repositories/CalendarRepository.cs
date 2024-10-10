﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    [CollectionName("calendars")]
    internal sealed class CalendarRepository : BaseRepository<Calendar>
    {
        public CalendarRepository(IMongoDatabase database, string instanceName, string? collectionPrefix = null)
            : base(database, instanceName, collectionPrefix)
        {
        }

        public async Task<bool> CalendarExists(string calendarName)
        {
            return
                await Collection.Find(
                    FilterBuilder.Where(calendar => calendar.Id == new CalendarId(calendarName, InstanceName))).AnyAsync().ConfigureAwait(false);
        }

        public async Task<Calendar> GetCalendar(string calendarName)
        {
            return
                await Collection.Find(calendar => calendar.Id == new CalendarId(calendarName, InstanceName)).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetCalendarNames()
        {
            return await Collection.Distinct(calendar => calendar.Id.CalendarName,
                calendar => calendar.Id.InstanceName == InstanceName)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<long> GetCount()
        {
            return await Collection.Find(calendar => calendar.Id.InstanceName == InstanceName)
                .CountDocumentsAsync()
                .ConfigureAwait(false);
        }

        public async Task AddCalendar(Calendar calendar)
        {
            await Collection.InsertOneAsync(calendar).ConfigureAwait(false);
        }

        public async Task<long> UpdateCalendar(Calendar calendar)
        {
            var result = await Collection.ReplaceOneAsync(cal => cal.Id == calendar.Id, calendar).ConfigureAwait(false);
            return result.MatchedCount;
        }

        public async Task<long> DeleteCalendar(string calendarName)
        {
            var result =
                await Collection.DeleteOneAsync(calendar => calendar.Id == new CalendarId(calendarName, InstanceName)).ConfigureAwait(false);
            return result.DeletedCount;
        }
    }
}
