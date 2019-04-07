using System;
using Microsoft.EntityFrameworkCore;
using Parking.Data.Entites.Statistic;

namespace Parking.Data.Api
{
    public interface IStatisticDataContext : IDataContext
    {
        DbSet<Record> Records {get;set;}
    }
}