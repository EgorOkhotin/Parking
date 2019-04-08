using System;
using Parking.Data.Entites.Statistic;

namespace Parking.Data.Api
{
    public interface IRecordFactory
    {
        Record CreateRecord(int incoming, int outcoming);
    }
}