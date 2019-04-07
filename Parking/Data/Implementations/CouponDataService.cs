using System;
using Parking.Data.Factories;
using Parking.Data.Api;
using Parking.Data.Entites;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Data.Implementations
{
    public class CouponDataService : ICouponDataService
    {
        ICouponDataContext _context;

        public CouponDataService([FromServices]ICouponDataContext context)
        {
            _context = context;
        }
        public async Task<Coupon> FindCoupon(string token)
        {
            if(token == null)
                throw new ArgumentNullException("Token can't be null");
            
            var c = await _context.Coupons.FirstOrDefaultAsync(x => x.Token == token);
            return c;
        }

        public async Task<bool> UseCoupon(Coupon coupon)
        {
            if(coupon == null)
                throw new ArgumentNullException("Coupon can't be null");
            
            if(coupon.Token == null)
                throw new ArgumentException("Coupon token can't be null");

            var c = await _context.Coupons.FirstOrDefaultAsync(x => x.Token == coupon.Token);

            if(c.IsUsed)
                throw new InvalidOperationException("Token already used!");
            
            c.IsUsed = true;
            _context.Coupons.Update(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}