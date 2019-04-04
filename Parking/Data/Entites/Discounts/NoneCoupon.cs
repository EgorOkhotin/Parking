using System;

namespace Parking.Data.Entites
{
    public class NoneCoupon : Coupon
    {
        public NoneCoupon()
        {
            this.CouponType = CouponType.None;
        }

        public override int GetDiscount(int cost)
        {
            return cost;
        }

        public override Coupon CreateInstance(Coupon coupon) 
        {
            if(coupon.CouponType != this.CouponType)
                throw new ArgumentException();

            return new NoneCoupon()
            {
                CouponType = this.CouponType,
                Token = this.Token,
                Name = coupon.Name,
                Id = coupon.Id,
                IsUsed = true
            };
        }
    }
}