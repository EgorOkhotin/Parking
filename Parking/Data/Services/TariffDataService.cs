using System;
using Parking.Data;
using Parking.Data.Entites;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Parking.Data.Api.Contexts;
using Parking.Data.Api.Services;

namespace Parking.Data.Services
{
    public class TariffDataService : ITariffDataService
    {
        ILogger<ITariffDataService> _logger;
        ITariffDataContext _context;
        public TariffDataService([FromServices] ITariffDataContext context,
        [FromServices] ILogger<ITariffDataService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<bool> Add(Tariff tariff)
        {
            try
            {
                _context.Tariffs.Add(tariff);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
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