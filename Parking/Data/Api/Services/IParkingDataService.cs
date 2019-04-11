using System;
using System.Linq;
using Parking.Data.Entites;

namespace Parking.Data.Api.Services
{
    public interface IParkingDataService
    {
        IQueryable<Data.Entites.Parking> GetParkingAreas();
    }
}