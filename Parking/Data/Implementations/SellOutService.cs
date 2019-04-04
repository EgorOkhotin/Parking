using System;
using Parking.Data.Factories;
using Parking.Data.Api;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Implementations
{
    public class SellOutService : ISellOutService
    {
        ISellOutDataContext _context;
        public Task<SellOut> FindSellOut(string tariffName)
        {
            throw new NotImplementedException();
        }
    }
}