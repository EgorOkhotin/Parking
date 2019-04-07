using System;
using Xunit;
using Parking.Controllers;
using Parking.Data;
using Parking;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Moq;
using Parking.Services.Api;
using Parking.Services.Implementations;
using Microsoft.Extensions.Logging;
using Parking.Data.Api;
using Parking.Data.Entites;



namespace ParkingTests
{
    public class EnterServiceTest
    {
        const int DEFAULT_COST = 10;
        EnterService _enterService;
        Mock<ILogger<IEnterService>> _enterServiceLogger;
        Mock<IKeyDataService> _keyService;
        Mock<ITariffDataService> _tariffService;

        Mock<IEntityFactory> _entityFactory;
        Mock<IModelCreateService> _modelCreateService;
        Mock<ICostCalculation> _costCalculationService;
        DateTimeBuilder _timeBuilder;
        ITestOutputHelper _output;

        public EnterServiceTest(ITestOutputHelper output)
        {
            _output = output;
            _timeBuilder = new DateTimeBuilder();
            _enterServiceLogger = new Mock<ILogger<IEnterService>>();
            
        }

        [Theory]
        [InlineData("LOW")]
        [InlineData("HIGH")]
        [InlineData("MIDDLE")]
        [InlineData(null)]
        public async Task EnterAnonymous_CorrectTariffName_NotNull(string tariff)
        {
            
            SetUpTariffService();
            SetUpKeyService();

            SetUpDataProperties();
            SetUpCalculationService();
            SetUpLogger();

            //_enterService = new EnterService(_keyService.Object, _tariffService.Object, _dataProperties.Object, _keyFactory.Object, _costCalculationService.Object, _enterServiceLogger.Object);

            var key = await _enterService.EnterForAnonymous(tariff);
            
            Assert.NotNull(key);
        }

        [Theory]
        [InlineData("LOW")]
        [InlineData("HIGH")]
        [InlineData("MIDDLE")]
        [InlineData(null)]
        public async Task EnterUser_CorrectTariffName_NotNull(string tariff)
        {
            SetUp();
            var k = await _enterService.EnterForAuthorize(tariff);
            Assert.NotNull(k);
        }

        [Theory]
        [InlineData("autoId1")]
        [InlineData("autoId2")]
        public async Task EnterUser_AutoId_NotNull(string autoId)
        {
            SetUp();
            var k = await _enterService.EnterForAuthorizeByAutoId(autoId);
            Assert.NotNull(k);
        }

        [Theory]
        [InlineData("autoId1", "tariff1")]
        [InlineData("autoId2", "tariff2")]
        public async Task EnterUser_ValidAutoIdTariff_NotNull(string autoId, string tariff)
        {
            SetUp();
            var k = await _enterService.EnterForAuthorizeByAutoId(autoId, tariff);
            Assert.NotNull(k);
        }

        [Theory]
        [InlineData("Unexist1")]
        [InlineData("Unexist2")]
        public async Task EnterUser_InvalidTariff_Throw(string tariff)
        {
            SetUp();

            _tariffService = new Mock<ITariffDataService>();
            var returned = Task.Run(()=> default(Tariff));
            _tariffService.Setup(t => t.GetByName(It.IsNotNull<string>())).Returns(returned);

            SetUpEnterService();

            var exceptionType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exceptionType, async()=>{await _enterService.EnterForAuthorize(tariff);});
        }

        [Theory]
        [InlineData("Unexist")]
        [InlineData("Unexist2")]
        public async Task Enter_InvalidTariff_Throw(string tariff)
        {
            SetUp();
            _tariffService = new Mock<ITariffDataService>();
            var returned = Task.Run(()=> default(Tariff));
            _tariffService.Setup(t => t.GetByName(It.IsNotNull<string>())).Returns(returned);
            SetUpEnterService();
            
            var exceptionType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exceptionType, async()=>{await _enterService.EnterForAnonymous(tariff);});
        }

        [Theory]
        [InlineData("Unexist")]
        [InlineData("Unexist2")]
        public async Task EnterById_InvalidTariff_Throw(string tariff)
        {
            SetUp();
            _tariffService = new Mock<ITariffDataService>();
            var returned = Task.Run(()=> default(Tariff));
            _tariffService.Setup(t => t.GetByName(It.IsNotNull<string>())).Returns(returned);
            SetUpEnterService();
            
            var exceptionType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exceptionType, async()=>{await _enterService.EnterForAuthorizeByAutoId("auto id", tariff);});
        }

        [Theory]
        [InlineData("token1")]
        [InlineData("token2")]
        public async Task GetCost_NullAtoId_ValidToken_ReturnCost(string token)
        {
            SetUp();
            Assert.True(await _enterService.GetCost(null,token)==DEFAULT_COST);
        }

        [Theory]
        [InlineData("token1", "auto1")]
        [InlineData("token2", "auto2")]
        public async Task GetCost_ValidAtoId_ValidToken_ReturnCost(string token, string autoId)
        {
            SetUp();
            Assert.True(await _enterService.GetCost(autoId,token)==DEFAULT_COST);
        }

        [Theory]
        [InlineData("auto1")]
        [InlineData("auto2")]
        public async Task GetCost_ValidAtoId_NullToken_ReturnCost(string autoId)
        {
            SetUp();
            Assert.True(await _enterService.GetCost(autoId,null)==DEFAULT_COST);
        }

        [Fact]
        public async Task GetCost_NullAtoId_NullToken_Throws()
        {
            SetUp();
            var exType = new ArgumentNullException().GetType();
            await Assert.ThrowsAsync(exType,async ()=>await _enterService.GetCost(null,null));
        }

        [Theory]
        [InlineData("token1")]
        [InlineData("token2")]
        public async Task Leave_ValidToken_ValidCost_True(string token)
        {
            SetUp();
            var r = await _enterService.Leave(token, DEFAULT_COST);
            Assert.True(r);
        }

        [Theory]
        [InlineData(null)]
        public async Task Leave_NullToken_ValidCost_Throws(string token)
        {
            SetUp();
            var exType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exType,async ()=>await _enterService.Leave(token, DEFAULT_COST));
        }

        [Theory]
        [InlineData("token")]
        public async Task Leave_ValidToknen_InvalidCost_Throws(string token)
        {
            SetUp();
            var exType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exType,async ()=>await _enterService.Leave(token, -DEFAULT_COST));
        }

        [Theory]
        [InlineData(null)]
        public async Task Leave_NullToknen_InvalidCost_Throws(string token)
        {
            SetUp();
            var exType = new ArgumentException().GetType();
            await Assert.ThrowsAsync(exType,async ()=>await _enterService.Leave(token, -DEFAULT_COST));
        }

        private void SetUpDataProperties()
        {
            //_dataProperties = new Mock<IDataProperties>();
            //_dataProperties.Setup(d => d.GetDefaultTariffName()).Returns("LOW");
        }

        private void SetUpCalculationService()
        {
            _costCalculationService = new Mock<ICostCalculation>();
            _costCalculationService.Setup(cost => cost.GetCost(It.IsAny<DateTime>(), It.Is<int>(n => n>=0))).Returns(DEFAULT_COST);
        }

        private void SetUpLogger()
        {
            _enterServiceLogger = new Mock<ILogger<IEnterService>>();
        }

        private void SetUpKeyService()
        {
            var defaultVal = Task.Run(()=>true);
            var returnedKey = Task.Run(()=>{
                return new Key(){
                    Token = "",
                    TimeStamp = default(DateTime),
                    TariffId = 0,
                    Tariff = new Tariff(){
                        Name = "",
                        Cost = DEFAULT_COST
                    }
                };
            });
            _keyService = new Mock<IKeyDataService>();
            _keyService.Setup(s => s.Add(It.IsNotNull<Key>())).Returns(defaultVal);
            _keyService.Setup(s => s.Delete(It.IsNotNull<string>())).Returns(defaultVal);
            _keyService.Setup(s => s.GetByToken(It.IsNotNull<string>())).Returns(returnedKey);
            _keyService.Setup(s => s.GetByAutoId(It.IsNotNull<string>())).Returns(returnedKey);
        }
        private void SetUpTariffService()
        {
            var defaultTariff = Task.Run(()=> new Tariff(){Name = It.IsNotNull<string>(), Cost=DEFAULT_COST });
            _tariffService = new Mock<ITariffDataService>();
            _tariffService.Setup(t => t.GetByName(It.IsNotNull<string>())).Returns(defaultTariff);
            _tariffService.Setup(t => t.GetById(It.Is<int>(i => i>=0))).Returns(defaultTariff);
        }

        private void SetUpKeyFactory()
        {
            // _keyFactory = new Mock<IKeyFactory>();
            // _keyFactory.Setup(f => f.CreateKey(It.IsNotNull<Tariff>())).Returns(new Key(){
            //     Token = It.IsNotNull<string>(),
            //     TimeStamp = _timeBuilder.GetTimeBeforeMinutes(10),
            //     AutoId = It.IsNotNull<string>(),
            //     Tariff = new Tariff(){
            //         Name = It.IsNotNull<string>(),
            //         Cost = It.Is<int>(n => n>=0)
            //     }
            // }); 
            // _keyFactory.Setup(f => f.CreateKey(It.IsNotNull<Tariff>(), It.IsNotNull<string>())).Returns(new Key(){
            //     Token = It.IsNotNull<string>(),
            //     TimeStamp = _timeBuilder.GetTimeBeforeMinutes(10),
            //     AutoId = It.IsNotNull<string>(),
            //     Tariff = new Tariff(){
            //         Name = It.IsNotNull<string>(),
            //         Cost = It.Is<int>(n => n>=0)
            //     }
            // });
        }

        private void SetUpEnterService()
        {
            // _enterService = new EnterService(_keyService.Object,
            // _tariffService.Object,
            // _dataProperties.Object,
            // _keyFactory.Object,
            // _costCalculationService.Object,
            // _enterServiceLogger.Object);
        }

        private void SetUp()
        {
            SetUpKeyFactory();
            SetUpTariffService();
            SetUpKeyService();
            SetUpDataProperties();
            SetUpCalculationService();
            SetUpLogger();
            SetUpEnterService();
        }
    }
}