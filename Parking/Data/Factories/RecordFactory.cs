using System;
using Parking.Data.Entites.Statistic;
using Parking.Data.Api;

namespace Parking.Data
{
    public class RecordFactory : IRecordFactory
    {
        public Record CreateRecord(int nonFreePlaces, AreaType area)
        {
            return new Record()
            {
                Area = area,
                Time = DateTime.Now,
                Description = $"Non free places: {nonFreePlaces}"
            };
        }
    }
}