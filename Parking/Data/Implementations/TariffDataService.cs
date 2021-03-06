using System;
using Parking.Data;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace Parking.Data.Implementations
{
    public class TariffDataService : ITariffService
    {
        ILogger<ITariffService> _logger;
        ITariffDataContext _context;
        public TariffDataService([FromServices] ITariffDataContext context,
        [FromServices] ILogger<ITariffService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<bool> Add(Tariff tariff)
        {
            try
            {
                _context.Tariffs.Attach(tariff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Can't add tariff! {tariff}");
                return false;
            }
        }

        public async Task<Tariff> GetById(int id)
        {
            return await GetTariff(x => x.Id == id, $"ID:{id}");
        }

        public async Task<Tariff> GetByName(string name)
        {
            return await GetTariff(x => x.Name == name, name);
        }

        public async Task<bool> Remove(string name)
        {
            var t = await GetByName(name);
            if(t == null) return false;
            _context.Tariffs.Remove(t);
            return true;
        }

        private async Task<Tariff> GetTariff(Expression<Func<Tariff,bool>> ex, string keyWord)
        {
            var t = await _context.Tariffs.FirstOrDefaultAsync(ex);
            if(t == null)
                _logger.LogWarning($"{GetType().Name}: Tarrif with didn't found! Search keyword: {keyWord}");
            return t;
        }
    }
}