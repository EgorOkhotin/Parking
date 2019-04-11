using System;
using Parking.Data.Factories;
using Parking.Data.Api;
using System.Threading.Tasks;
using Parking.Data.Entites;
using Microsoft.AspNetCore.Mvc;
using Parking.Data.Api.Contexts;
using Parking.Data.Api.Services;

namespace Parking.Data.Services
{
    public class DiscountDataService : IDiscountDataService
    {
        ISubscriptionDataService _subscriptions;
        ICouponDataService _coupons;
        ISellOutDataService _sellOuts;

        public DiscountDataService([FromServices]ISubscriptionDataService subscriptions,
        [FromServices]ICouponDataService coupons,
        [FromServices]ISellOutDataService sellOuts)
        {
            _subscriptions = subscriptions;
            _coupons = coupons;
            _sellOuts = sellOuts;
        }
        public async Task<Discount> FindSellOut(string tariffName)
        {
            return await _sellOuts.FindSellOut(tariffName);
        }

        public async Task<Discount> FindSubscription(string userEmail, string tariff)
        {
            var subscription = await _subscriptions.FindSubscription(userEmail);
            if(subscription != null && subscription.TariffNames.Contains(tariff))
            {
                return subscription;
            }
            return null;
        }

        public async Task<Discount> FindCoupon(string couponToken)
        {
            return await _coupons.FindCoupon(couponToken);
        }

        public async Task UseCoupon(Discount coupon)
        {
            Coupon c = null;
            try
            {
                c = (Coupon)coupon;
            }
            catch(InvalidCastException)
            {
                throw new  InvalidCastException("Incorrect coupon type!");
            }
            await _coupons.UseCoupon(c);
        }

        public async Task UseSellOut(Discount sellOut)
        {
            SellOut s = null;
            try{
                s = (SellOut)sellOut;
            }
            catch(InvalidCastException)
            {
                throw new InvalidCastException("Incorrect sellout type");
            }

            await _sellOuts.UseSellOut(s);
        }
    }
}