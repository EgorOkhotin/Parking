using Microsoft.EntityFrameworkCore;
using Parking.Data.Entites;
using System;

namespace Parking.Data.Api.Contexts
{
    public interface IParkingDataContext : IDataContext
    {
        DbSet<Data.Entites.Parking> Parkings {get;set;}
    }
}