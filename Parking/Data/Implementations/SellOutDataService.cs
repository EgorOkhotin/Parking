using System;
using Parking.Data.Factories;
using Parking.Data.Api;
using Parking.Data.Entites;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Data.Implementations
{
    public class SellOutDataService : ISellOutDataService
    {
        ISellOutDataContext _context;

        public SellOutDataService([FromServices]ISellOutDataContext context)
        {
            _context = context;
        }
        public async Task<SellOut> FindSellOut(string tariffName)
        {
            if (tariffName == null)
                throw new ArgumentNullException("Tariff name can't be null");

            var sellout = await Task.Run(() =>
            {
                return _context.SellOuts
                .Where(x => x.Start > DateTime.Now && x.End < DateTime.Now)
                .Where(x => x.Counter > 0)
                .SingleOrDefault(x => x.Tariffs.Contains(tariffName));
            });

            return sellout;
        }

        public async Task<bool> UseSellOut(SellOut sellOut)
        {
            if(sellOut == null)
                throw new ArgumentNullException("Sellout can't be null");
            
            if(sellOut.Id > 0)
                throw new ArgumentException("Sellout id must be initialized");

            var s = _context.SellOuts.First(x => x.Id == sellOut.Id);
            if(s.Counter <= 0)
                throw new InvalidOperationException("Sellout is ended");
            s.Counter--;
            _context.SellOuts.Update(s);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}