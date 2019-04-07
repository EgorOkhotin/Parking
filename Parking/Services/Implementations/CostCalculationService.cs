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
        ITariffDataService _tariffs;
        
        IDiscountService _discounts;
        private readonly ILogger<ICostCalculation> _logger;
        public CostCalculationService([FromServices] IConfiguration configuration,
        [FromServices] ITariffDataService tariffService,
        [FromServices] IDiscountService discounts,
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

        public int GetCost(Key key, string userEmail =null, string coupon =  null)
        {
            var tariff = GetTariff(key.TariffId).Result;
            int cost = tariff.Cost;
            cost = GetCost(key.TimeStamp, cost);

            try
            {
                cost = GetDiscount(cost, tariff.Name, userEmail, coupon).Result;
            }
            catch(ArgumentException ex)
            {
                _logger.LogError($"COST CALCULATION: Can't use discount! {ex.Message}");
            }
            
            return cost;
        }

        private async Task<Tariff> GetTariff(int? tariffId)
        {
            return await _tariffs.GetById(tariffId.Value);
        }

        private async Task<int> GetDiscount(int cost, string tariffName, string userEmail, string coupon)
        {
            var res = await _discounts.GetCost(cost, tariffName, userEmail, coupon);
            return res;
        }

        private int GetIntervalsCount(TimeSpan span)
        {
            return ((int)span.TotalMinutes)/_timeInterval;
        }
    }
}
