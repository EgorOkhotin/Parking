using System;
using Parking.Data.Entites.Statistic;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Parking.Data.Api
{
    public interface IStatisticDataService
    {
        Task AddRecord(int incoming, int outcoming);
        Task<IEnumerable<Record>> GetRecordsByDate(DateTime date);
        Task<IEnumerable<Record>> GetRecordsBetweenDates(DateTime start, DateTime end);
    }
}