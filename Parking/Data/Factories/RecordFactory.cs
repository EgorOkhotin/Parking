using System;
using Parking.Data.Entites.Statistic;
using Parking.Data.Api.Factories;
using Parking.Data.Api.Services;
using Parking.Data.Api;

namespace Parking.Data
{
    public class RecordFactory : IRecordFactory
    {
        public Record CreateRecord(int incoming, int outcoming)
        {
            return new Record()
            {
                Time = DateTime.Now,
                Description = $"Incoming: ({incoming}) \t Outcoming: ({outcoming})"
            };
        }
    }
}