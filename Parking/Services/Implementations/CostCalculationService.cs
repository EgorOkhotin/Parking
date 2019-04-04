using Parking.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Parking.Data.Entites;
using Parking.Data.Api;

namespace Parking.Services.Implementations
{
    public class CostCalculationService : ICostCalculation
    {
        private readonly int _timeInterval;
        private readonly int _freeTimeInterval;
        ITariffService _tariffs;
        IDiscountDataService _discounts;
        private readonly ILogger<ICostCalculation> _logger;
        public CostCalculationService([FromServices] IConfiguration configuration,
        [FromServices] ITariffService tariffService,
        [FromServices] IDiscountDataService discounts,
        [FromServices] ILogger<ICostCalculation> logger)
        {
            _logger = logger;
            var interval = configuration.GetValue<int>("Tariffs:CostTimeInteval");
            var freeTimeInterval = configuration.GetValue<int>("Tariffs:FreeTimeInteval");
            _tariffs = tariffService;
            _discounts = discounts;

            _logger.LogInformation($"Cost-Time Interval: {interval}");
            _logger.LogInformation($"Free time Interval: {freeTimeInterval}");
            
            if(interval <= 0 || freeTimeInterval < 0)
            {
                _logger.LogError("Cost per time interval was not found in configuration file.\n" + 
                    "\tSet valid time interval and reboot application.\n"+
                    "\tInterval must be in minutes");
                
                throw new SystemException("Incorrect configuration!");
            }
            _timeInterval = interval;
            _freeTimeInterval = freeTimeInterval;
        }
        //TODO Implement cost calculation
        public int GetCost(DateTime timeStamp, int cost)
        {
            var now = DateTime.Now;
            if( now < timeStamp) throw new ArgumentException("Incorrect time stamp! This parametr must be less than current time");
            if(cost < 0) throw new ArgumentException("Cost can't be less than zero!");
            var difference = now - timeStamp;
            if(difference.TotalMinutes <= _freeTimeInterval) return 0;
            var result = cost*GetIntervalsCount(difference);
            if(result < 0)
            {
                _logger.LogError($"Calculated cost less than zero!\n\t Time: {timeStamp}\n\t Cost: {cost}");
                return 0;
            }
            else return result;
        }

        public int GetCost(Key key)
        {
            var tariff = GetTariff(key.TariffId).Result;
            int cost = tariff.Cost;

            throw new NotImplementedException();
        }

        private bool IsHaveDiscount(Key k)
        {
            throw new NotImplementedException();
        }

        private async Task<Tariff> GetTariff(int? tariffId)
        {
            return await _tariffs.GetById(tariffId.Value);
        }

        private int GetIntervalsCount(TimeSpan span)
        {
            return ((int)span.TotalMinutes)/_timeInterval;
        }
    }
}
