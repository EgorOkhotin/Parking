using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data.Entites;
using System.Threading;

namespace Parking.Data.Api
{
    public interface ISellOutDataContext : IDataContext
    {
        DbSet<SellOut> SellOuts {get;set;}
    }
}