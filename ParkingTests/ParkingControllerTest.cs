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

namespace ParkingTests
{
    public class ParkingControllerTest
    {
        const string TARIFF_NAME = "TEST_NAME";
        const string USER_NAME = "TEST_USER";
        const string USER_ROLE = "user";
        const string EMPLOYEE_ROLE = "employee";
        ParkingController _controller {get;set;}
        ITestOutputHelper _output;
        DefaultHttpContext _context;
        ClaimsBuilder _userBuilder;
        Logger<ParkingController> _logger;
        public ParkingControllerTest(ITestOutputHelper helper)
        {
            _output = helper;
            //_output.WriteLine("Created!");
            var costCalculation = (new CostCalculationServiceTest()).GetCalculationService();
            _logger = new Logger<ParkingController>();
            _controller = new ParkingController(new DataAdapterMock(), costCalculation, _logger);
            _context = new DefaultHttpContext();
            
            _controller.ControllerContext.HttpContext = _context;
            _userBuilder = new ClaimsBuilder();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(USER_ROLE)]
        [InlineData(EMPLOYEE_ROLE)]
        public async Task Enter_TariffName_NotNull(string role)
        {
            Assert.True(await Enter(role, TARIFF_NAME));
        }

        private async Task<bool> Enter(string role, string tariff)
        {   
            SetUpUser(role);    
            
            var k = await _controller.Enter(tariff);

            return IsEntered(k,role);
        }

        private void SetUpUser(string role)
        {
            if(role != null)
                _context.User = _userBuilder.CreateUser(USER_NAME, role);
        }

        private bool IsEntered(Parking.Models.Key key, string role)
        {
            if(role == null)
                return (key != null);

            return (key!=null) && _logger.ContainsRecord(role);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(USER_ROLE)]
        [InlineData(EMPLOYEE_ROLE)]
        public async Task Enter_NullTariff_Null(string role)
        {
            Assert.False(await Enter(role,null));
        }

        [Fact]
        public async void GetCost_CorrectToken_GetValue()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var result = await _controller.GetCost(null, k.Token);
            Assert.True(result>0);
        }

        [Fact]
        public async void GetCost_NullToken_Null()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var r = await _controller.GetCost(null);
            Assert.Null(r);
        }

        [Fact]
        public async Task GetPay_CorrectToken_GetValue()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var cost = await _controller.GetCost(null, k.Token);
            var result = await _controller.GetPay(k.Token, cost.Value);
            Assert.True(result);
        }

        [Fact]
        public async Task GetPay_NullToken_False()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var cost = await _controller.GetCost(null, k.Token);
            var result = await _controller.GetPay(null, cost.Value);
            Assert.False(result);
        }

        [Fact]
        public async Task GetPay_LessZeroCost_False()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var cost = await _controller.GetCost(null, k.Token);
            var result = await  _controller.GetPay(k.Token,-cost.Value);
            Assert.False(result);
        }

        [Fact]
        public async Task EnterByAutoId_NotNullAutoId_ReturnToken()
        {
            var k = await _controller.Enter(TARIFF_NAME, "a123oo73");
            Assert.NotNull(k);
        }
    }
}