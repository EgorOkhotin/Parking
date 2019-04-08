using System;
using System.Linq;
using Parking.Data.Entites;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Data.Implementations
{
    public class ParkingDataService : IParkingDataService
    {
        IParkingDataContext _context;
        public ParkingDataService([FromServices] IParkingDataContext context)
        {
            _context = context;
        }
        public IQueryable<Data.Entites.Parking> GetParkingAreas()
        {
            return _context.Parkings;
        }
    }
}