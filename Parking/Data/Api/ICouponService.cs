using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface ICouponService
    {
        Task<Coupon> FindCoupon(string token);
        Task<bool> UseCoupon(Coupon coupon);
        
    }
}