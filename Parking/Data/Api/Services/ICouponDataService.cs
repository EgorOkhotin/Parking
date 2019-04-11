using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api.Services
{
    public interface ICouponDataService
    {
        Task<Coupon> FindCoupon(string token);
        Task<bool> UseCoupon(Coupon coupon);
        
    }
}