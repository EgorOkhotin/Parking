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
        ParkingController _controller {get;set;}
        ITestOutputHelper _helper;
        public ParkingControllerTest(ITestOutputHelper helper)
        {
            _helper = helper;
            var costCalculation = (new CostCalculationServiceTest()).GetCalculationService();
            _controller = new ParkingController(new DataAdapterMock(), costCalculation, new Logger<ParkingController>());
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task Enter_TestName_NotNull()
        {
            
            var k = await _controller.Enter(TARIFF_NAME);
            Assert.NotNull(k);
        }

        [Fact]
        public async Task EnterUser_TestName_NotNull()
        {
            
            var k = await _controller.EnterUser(TARIFF_NAME);
            var r = (k!=null);
            Assert.True(r);
        }

        [Fact]
        public async Task EnterEmployee_TestName_NotNull()
        {
            
            var k = await _controller.EnterEmployee(TARIFF_NAME);
            Assert.NotNull(k);
        }

        [Fact]
        public async Task Enter_NullName_Throws()
        {
            var k = await _controller.Enter(null);
            Assert.Null(k);
        }

        [Fact]
        public async Task EnterUser_NullName_Throws()
        {
            var k = await _controller.EnterUser(null);
            Assert.Null(k);
        }

        [Fact]
        public async Task EnterEmployee_NullName_Throws()
        {
            var k = await _controller.EnterEmployee(null);
            Assert.Null(k);
        }

        [Fact]
        public async void GetCost_CorrectToken_GetValue()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var result = await _controller.GetCost(k.Token);
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
            var cost = await _controller.GetCost(k.Token);
            var result = await _controller.GetPay(k.Token, cost.Value);
            Assert.True(result);
        }

        [Fact]
        public async Task GetPay_NullToken_False()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var cost = await _controller.GetCost(k.Token);
            var result = await _controller.GetPay(null, cost.Value);
            Assert.False(result);
        }

        [Fact]
        public async Task GetPay_LessZeroCost_False()
        {
            var k = await _controller.Enter(TARIFF_NAME);
            var cost = await _controller.GetCost(k.Token);
            var result = await  _controller.GetPay(k.Token,-cost.Value);
            Assert.False(result);
        }

        [Fact]
        public async Task EnterByAutoId_NotNullAutoId_ReturnToken()
        {
            var k = await _controller.EnterByAutoId(TARIFF_NAME, "a123oo73");
            Assert.NotNull(k);
        }
    }
}