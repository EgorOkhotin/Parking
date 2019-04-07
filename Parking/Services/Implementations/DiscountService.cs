using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parking.Data.Api;
using Parking.Data.Entites;
using Parking.Services.Api;

namespace Parking.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        IDiscountDataService _data;
        public DiscountService([FromServices] IDiscountDataService data)
        {
            _data = data;
        }

        public Task<int> GetCost(int cost, string tariffName, string userEmail, string coupon)
        {
            if(cost < 0 || tariffName == null)
                throw new ArgumentException("Input parametrs are wrong");

            if(coupon != null)
                return GetCostByCoupon(cost, tariffName, coupon);
            
            if(userEmail != null)
                return GetCostBySubscription(cost, tariffName, userEmail);

            return GetCostBySellOut(cost, tariffName);

            throw new NotImplementedException();
        }

        private async Task<int> GetCostByCoupon(int cost, string tariff, string coupon)
        {
            var c = await _data.FindCoupon(coupon);
            if(c == null)
                throw new ArgumentException("Coupon don't exist");
            
            if(!c.IsActive())
                return cost;

            var resultCost = c.GetDiscount(cost);
            await _data.UseCoupon(c);
            return resultCost;
        }

        private async Task<int> GetCostBySellOut(int cost, string tariff)
        {
            var s = await _data.FindSellOut(tariff);
            if(s == null)
                return cost;

            if(!s.IsActive())
                return cost;
            
            var resultCost = s.GetDiscount(cost);
            await _data.UseSellOut(s);
            
            return resultCost;
        }

        private async Task<int> GetCostBySubscription(int cost, string tariff, string email)
        {
            var s = await _data.FindSubscription(email, tariff);
            if(s == null)
                return cost;

            if(!s.IsActive())
                return cost;
            
            return s.GetDiscount(cost);
        }
    }
}