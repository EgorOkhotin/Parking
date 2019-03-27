using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data;
using System.Threading;

namespace Parking.Data.Api
{
    public interface ITariffDataContext : IDataContext
    {
        DbSet<Tariff> Tariffs {get;}
    }
}