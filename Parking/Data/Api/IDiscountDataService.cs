using System;
using System.Threading.Tasks;
using Parking.Data.Entites;

namespace Parking.Data.Api
{
    public interface IDiscountDataService
    {
        Task<Discount> FindCoupon(string couponToken);
        Task UseCoupon(Discount coupon);
        Task<Discount> FindSellOut(string tariffName);
        Task UseSellOut(Discount sellOut);
        Task<Discount> FindSubscription(string userEmail, string tariffName);
    }
}