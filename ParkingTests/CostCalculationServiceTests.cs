using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions;
using Parking.Services.Implementations;
using Parking.Services.Api;
using System.Collections.Generic;
using Xunit.Abstractions;
using Moq;
using Microsoft.Extensions.Logging;
using Parking.Data.Api;
using System.Threading.Tasks;
using Parking.Data.Entites;

namespace ParkingTests
{
    public class CostCalculationServiceTest
    {
        IConfiguration _configuration;
        Mock<ILogger<ICostCalculation>> _logger;
        Mock<IDiscountService> _discounts;
        Mock<ITariffDataService> _tariffs;
        ICostCalculation _cost;
        DateTimeBuilder TimeBuilder { get; set; }

        ITestOutputHelper _output;

        const int LOW_COST = 10;
        const int MIDDLE_COST = 20;
        const int HIGH_COST = 30;
        const int SPECIAL_COST = 0;
        const int DEFAULT_COST = 10;


        const int COST_INTEVAL = 20;
        const int FREE_INTERVAL = COST_INTEVAL/2;

        public CostCalculationServiceTest(ITestOutputHelper output)
        {      
            TimeBuilder = new DateTimeBuilder();
            _configuration = GetConfiguration(GetConfigurationSource(InitializeIntervals()));
            _logger = new Mock<ILogger<ICostCalculation>>();
            _discounts = InitializeDiscountService();
            _tariffs = InitializeTariffDataService();
            _cost = new CostCalculationService(_configuration, _tariffs.Object, _discounts.Object, _logger.Object);
            _output = output;
        }
        
        [Theory]
        [InlineData(LOW_COST)]
        [InlineData(MIDDLE_COST)]
        [InlineData(HIGH_COST)]
        [InlineData(SPECIAL_COST)]
        public void GetCost_FewTimeInterval_ReturnTrue(int tariff)
        {
            var k = new Random().Next(2,15);
            var cost = GetFewIntervalsCost(tariff, k);
            Assert.True(cost == tariff * k);
        }

        [Theory]
        [InlineData(LOW_COST)]
        [InlineData(MIDDLE_COST)]
        [InlineData(HIGH_COST)]
        [InlineData(SPECIAL_COST)]
        public void GetCost_FutureTime_ThrowException(int tariff)
        {
            var k = new Random().Next(2,15);
            
            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetFutureTimeCost(tariff, k);
            });
        }
        
        [Theory]
        [InlineData(LOW_COST)]
        [InlineData(MIDDLE_COST)]
        [InlineData(HIGH_COST)]
        public void GetCost_LessZeroCost_ThrowsException(int tariff)
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(),()=>{
                var cost = GetCostByLessZeroPrice(tariff, k);
            });
        }
        
        [Theory]
        [InlineData(LOW_COST)]
        [InlineData(MIDDLE_COST)]
        [InlineData(HIGH_COST)]
        public void GetCost_LessZeroCostAndFutureTime_ThrowsException(int tariff)
        {
            var k = new Random().Next(2,15);

            Assert.Throws(new ArgumentException().GetType(), ()=>{
                var cost = GetCostByLessZeroPriceAndFutureTime(tariff, k);
            });
        }

        [Fact]
        public void GetCost_ValidKey_ReturnCost()
        {
            var coff = new Random().Next(2,15);
            var time = TimeBuilder.GetTimeBeforeMinutes(COST_INTEVAL * coff);
            _output.WriteLine($"Builded time:{time}");

            var k = new Key(){
                TariffId = 1,
                Tariff = new Tariff(){
                    Id = 1,
                    Name = "Empty",
                    Cost = DEFAULT_COST
                },
                TimeStamp = TimeBuilder.GetTimeBeforeMinutes(COST_INTEVAL * coff)
            };

            _discounts = new Mock<IDiscountService>();
            _discounts.Setup(x => x.GetCost(It.IsAny<int>(), It.IsNotNull<string>(), null, null))
            .Returns(Task.Run(async ()=> {return coff*DEFAULT_COST;}));
            _output.WriteLine($"Returned cost {_discounts.Object.GetCost(0,"Some").Result}");
            _output.WriteLine($"Returned tariff {_tariffs.Object.GetById(1).Result}");
            _cost = new CostCalculationService(_configuration, _tariffs.Object, _discounts.Object, _logger.Object);

            var result = _cost.GetCost(k);

            _output.WriteLine($"Calculated cost:{result}");
            Assert.True((coff * DEFAULT_COST) == result);
        }

        public int GetCostByLessZeroPriceAndFutureTime(int cost, int k)
        {
            return GetCost(-cost, -COST_INTEVAL*k);
        }
        public int GetCostByLessZeroPrice(int cost, int k)
        {
            return GetCost(-cost, COST_INTEVAL*k);
        }

        public int GetFutureTimeCost(int cost, int k)
        {
            return GetCost(cost, -COST_INTEVAL * k);
        }

        private int GetFewIntervalsCost(int cost, int k)
        {
            return GetCost(cost, COST_INTEVAL*k);
        }

        private int GetCost(int cost, int interval)
        {
            var date = TimeBuilder.GetTimeBeforeMinutes(interval);
            return _cost.GetCost(date, cost);
        }

        private IEnumerable<KeyValuePair<string,string>> InitializeIntervals()
        {
            var interval = new Random().Next(10, 20);
            var interval1 = new KeyValuePair<string, string>("Tariffs:CostTimeInteval", COST_INTEVAL.ToString());
            var interval2 = new KeyValuePair<string, string>("Tariffs:FreeTimeInteval", FREE_INTERVAL.ToString());
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

        private Mock<IDiscountService> InitializeDiscountService()
        {
            var service = new Mock<IDiscountService>();
            var defaultCost = Task.Run(()=> DEFAULT_COST);
            service.Setup(x => x.GetCost(It.Is<int>(n => n>=0), It.IsNotNull<string>(), null, null)).Returns(defaultCost);
            return service;
        }

        private Mock<ITariffDataService> InitializeTariffDataService()
        {
            var defaultTariff = Task.Run(()=> new Tariff(){Name = "EMPTY", Cost=DEFAULT_COST });
            var service = new Mock<ITariffDataService>();
            service.Setup(x => x.GetById(It.IsNotNull<int>())).Returns(defaultTariff);
            return service;
        }
    }
}

