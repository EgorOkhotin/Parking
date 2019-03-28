using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions;
using Parking.Services.Implementations;
using Parking.Services.Api;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace ParkingTests
{
    public class CostCalculationServiceTest
    {
        IConfiguration Configuration {get;set;}
        DateTimeBuilder TimeBuilder { get; set; }
        ICostCalculation Service {get;set;}

        const int LOW_COST = 10;
        const int MIDDLE_COST = 20;
        const int HIGH_COST = 30;
        const int SPECIAL_COST = 0;

        int _interval;
        int _freeInterval;

        public CostCalculationServiceTest()
        {
            var source = GetConfigurationSource(InitializeIntervals());
            Configuration = GetConfiguration(source);
            TimeBuilder = new DateTimeBuilder();
            Service = new CostCalculationService(Configuration, new Logger<CostCalculationService>());
        }

        public ICostCalculation GetCalculationService()
        {
            return Service;
        }
        
        
        public void GetCost_Low_FewTimeInterval_ReturnTrue()
        {
            var k = new Random().Next(2,15);
            var cost = GetFewIntervalsCost(LOW_COST, k);
            Assert.True(cost == LOW_COST * k);
        }

        
        public void GetCost_Middle_FewInterval_ReturnTrue()
        {
            var k = new Random().Next(2,15);
            var cost = GetFewIntervalsCost(MIDDLE_COST, k);
            Assert.True(cost == MIDDLE_COST*k);
        }

        
        public void GetCost_High_FewInterval_ReturnTrue()
        {
            var k = new Random().Next(2,15);
            var cost = GetFewIntervalsCost(HIGH_COST, k);
            Assert.True(cost == HIGH_COST*k);
        }

        
        public void GetCost_Special_FewInterval_ReturnTrue()
        {
            var k = new Random().Next(2,15);
            var cost = GetFewIntervalsCost(SPECIAL_COST, k);
            Assert.True(cost == SPECIAL_COST*k);
        }

        
        public void GetCost_LOW_FutureTime_ThrowException()
        {
            var k = new Random().Next(2,15);
            
            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetFutureTimeCost(LOW_COST, k);
            });
        }

        
        public void GetCost_Middle_FutureTime_ThrowException()
        {
            var k = new Random().Next(2,15);
            
            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetFutureTimeCost(MIDDLE_COST, k);
            });
        }

        
        public void GetCost_Low_LessZeroCost_ThrowsException()
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetCostByLessZeroPrice(LOW_COST, k);
            });
        }

        
        public void GetCost_Middle_LessZeroCost_ThrowsException()
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetCostByLessZeroPrice(MIDDLE_COST, k);
            });
        }

        
        public void GetCost_Low_LessZeroCostAndFutureTime_ThrowsException()
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(), ()=>{
                var cost = GetCostByLessZeroPriceAndFutureTime(LOW_COST, k);
            });
        }

        
        public void GetCost_Middle_LessZeroCostAndFutureTime_ThrowsException()
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetCostByLessZeroPriceAndFutureTime(MIDDLE_COST, k);
            });
        }

        public int GetCostByLessZeroPriceAndFutureTime(int cost, int k)
        {
            return GetCost(-cost, -_interval*k);
        }
        public int GetCostByLessZeroPrice(int cost, int k)
        {
            return GetCost(-cost, _interval*k);
        }

        public int GetFutureTimeCost(int cost, int k)
        {
            return GetCost(cost, -_interval * k);
        }

        private int GetFewIntervalsCost(int cost, int k)
        {
            return GetCost(cost, _interval*k);
        }

        private int GetCost(int cost, int interval)
        {
            var date = TimeBuilder.GetTimeBeforeMinutes(interval);
            return Service.GetCost(date, cost);
        }

        private IEnumerable<KeyValuePair<string,string>> InitializeIntervals()
        {
            var interval = new Random().Next(10, 20);
            _interval = interval;
            _freeInterval = (interval / 2);
            var interval1 = new KeyValuePair<string, string>("CostTimeInteval", _interval.ToString());
            var interval2 = new KeyValuePair<string, string>("FreeTimeInteval", _freeInterval.ToString());
            return new List<KeyValuePair<string,string>>(){interval1, interval2};
        }

        private IConfigurationSource GetConfigurationSource(IEnumerable<KeyValuePair<string,string>> values)
        {
            var memoryConfigurationSource = new MemoryConfigurationSource();
            memoryConfigurationSource.InitialData = values;
            return memoryConfigurationSource;
        }

        private IConfiguration GetConfiguration(IConfigurationSource source)
        {
            return new ConfigurationBuilder()
                .Add(source)
                .Build();
        }
    }
}

