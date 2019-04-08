using System;
using System.Linq;
using Parking.Data.Entites;

namespace Parking.Data.Api
{
    public interface IParkingDataService
    {
        IQueryable<Data.Entites.Parking> GetParkingAreas();
    }
}