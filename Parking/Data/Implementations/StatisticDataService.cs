using System;
using System.Threading.Tasks;
using Parking.Data.Entites.Statistic;
using Parking.Data.Api;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Data.Implementations
{
    public class StatisticDataService : IStatisticDataService
    {
        IRecordFactory _recordFactory;
        IStatisticDataContext _context;

        public StatisticDataService([FromServices] IRecordFactory recordFactory,
        [FromServices] IStatisticDataContext context)
        {
            _recordFactory = recordFactory;
            _context = context;
        }
        public async Task AddRecord(int nonFreePlaces, AreaType area)
        {
            var record = _recordFactory.CreateRecord(nonFreePlaces, area);
            _context.Records.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Record>> GetRecordsBetweenDates(DateTime start, DateTime end)
        {
            return await Task.Run(()=>{
                var records = _context.Records.Where(x => (x.Time>=start && x.Time<=end));
                return records.AsEnumerable();
            });
        }

        public async Task<IEnumerable<Record>> GetRecordsByDate(DateTime date)
        {
            return await Task.Run(()=>{
                var records = _context.Records.Where(x => x.Time.Date == date.Date);
                return records.AsEnumerable();
            });
        }
    }
}