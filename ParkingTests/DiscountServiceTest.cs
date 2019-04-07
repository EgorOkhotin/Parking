using System;
using Parking.Data.Entites;
using Parking.Services.Api;
using Parking.Services.Implementations;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Moq;
using Parking.Data.Api;

namespace ParkingTests
{
    public class DiscountServiceTest
    {
        IDiscountService _discount;
        Mock<IDiscountDataService> _data;
        DateTimeBuilder _timeBuilder;
        ITestOutputHelper _output;
        const string TARIFF_NAME = "TEST";
        const string DISCOUNT_NAME = "EMPTY";
        public DiscountServiceTest(ITestOutputHelper output)
        {
            _timeBuilder = new DateTimeBuilder();
            _output = output;
        }

        [Fact]
        public async Task GetCost_ValidCoupon_ReturnCost()
        {
            SetUp(GetDefaultCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var cost = await _discount.GetCost(10, TARIFF_NAME, null, DISCOUNT_NAME);
            //_output.WriteLine($"Calculated cost: {cost}");
            var result = ((cost>=7) && (cost<10));
            Assert.True(result);
        } 

        [Fact]
        public async Task GetCost_ValidSubscription_ReturnCost()
        {
            SetUp(GetDefaultCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var cost = await _discount.GetCost(10, TARIFF_NAME, "USER", null);
            var result = (cost == 0);
            Assert.True(result);
        }

        [Fact]
        public async Task GetCost_SellOut_ReturnCost()
        {
            SetUp(GetDefaultCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var cost = await _discount.GetCost(10, TARIFF_NAME);
            var result = ((cost>=7) && (cost<10));
            Assert.True(result);
        }

        [Fact]
        public async Task GetCost_InvalidCost_Throws()
        {
            SetUp(GetDefaultCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var type = typeof(ArgumentException);
            await Assert.ThrowsAsync(type, async () => await _discount.GetCost(-10, TARIFF_NAME));
        }

        [Fact]
        public async Task GetCost_InvalidTariffName_Throws()
        {
            SetUp(GetDefaultCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var type = typeof(ArgumentException);
            await Assert.ThrowsAsync(type, async () => await _discount.GetCost(10, null));
        }

        [Fact]
        public async Task GetCost_InactiveCoupon_GetFullCost()
        {
            SetUp(GetInactiveCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var cost = await _discount.GetCost(10, TARIFF_NAME, null, DISCOUNT_NAME);
            var result = (cost==10);
            Assert.True(result);
        }

        [Fact]
        public async Task GetCost_NullCoupon_Throws()
        {
            SetUp(GetNullCoupon(), GetDefaultSellOut(), GetDefaultSubscription());
            var type = typeof(ArgumentException);
            await Assert.ThrowsAsync(type, async()=> await _discount.GetCost(10, TARIFF_NAME, null, DISCOUNT_NAME));
        }

        private void SetUp(Task<Discount> coupon, Task<Discount> sellOut, Task<Discount> subscription)
        {
            SetUpDiscountDataService(coupon, sellOut, subscription);
            _discount = new DiscountService(_data.Object);
        }

        private void SetUpDiscountDataService(Task<Discount> coupon, Task<Discount> sellOut, Task<Discount> subscription)
        {
            var defaultCoupon = GetDefaultCoupon();
            var defaultSellOut = GetDefaultSellOut();
            var defaultSubsciption = GetDefaultSubscription();
            _data = new Mock<IDiscountDataService>();
            _data.Setup(x => x.FindCoupon(It.IsNotNull<string>())).Returns(coupon);
            _data.Setup(x => x.FindSellOut(It.IsNotNull<string>())).Returns(sellOut);
            _data.Setup(x => x.FindSubscription(It.IsNotNull<string>(), It.IsNotNull<string>())).Returns(subscription);
        }

        private Task<Discount> GetDefaultCoupon()
        {
            return Task.Run(()=> {
                return (Discount)new Off25Coupon(){
                    CouponType = CouponType.Off25,
                    Name = "EMPTY",
                    IsUsed = false,
                    Id = 0,
                    Start = DateTime.MinValue,
                    End = DateTime.MaxValue
                };
            });
        }

        private Task<Discount> GetInactiveCoupon()
        {
            return Task.Run(()=>{
                return (Discount)new Off25Coupon(){
                    CouponType = CouponType.Off25,
                    Name = "EMPTY",
                    IsUsed = false,
                    Id = 0,
                    Start = _timeBuilder.GetTimeBeforeMinutes(20),
                    End = _timeBuilder.GetTimeBeforeMinutes(10)
                };
            });
        }

        private Task<Discount> GetNullCoupon()
        {
            return Task.Run(()=>{
                return (Discount)null;
            });
        }

        private Task<Discount> GetDefaultSellOut()
        {
            return Task.Run(()=> {
                return (Discount) new Off25SellOut(){
                    SellOutType = SellOutType.Off25,
                    Name = "EMPTY",
                    Counter = 100,
                    Id = 0,
                    Start = _timeBuilder.GetTimeBeforeMinutes(10),
                    End = _timeBuilder.GetTimeBeforeMinutes(-100),
                    Tariffs = "HIGH TEST"
                };
            });
        }

        private Task<Discount> GetDefaultSubscription()
        {
            return Task.Run(()=>{
                return (Discount) new Subscription(){
                    TariffNames = "HIGH TEST",
                    Id = 0,
                    Name = "EMPTY",
                    Start = _timeBuilder.GetTimeBeforeMinutes(10),
                    End = _timeBuilder.GetTimeBeforeMinutes(-100)
                };
            });
        }
    }
}