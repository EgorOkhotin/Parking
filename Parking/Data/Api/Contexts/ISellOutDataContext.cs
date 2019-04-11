using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data.Entites;
using System.Threading;

namespace Parking.Data.Api.Contexts
{
    public interface ISellOutDataContext : IDataContext
    {
        DbSet<SellOut> SellOuts {get;set;}
    }
}