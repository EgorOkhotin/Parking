using System;

namespace Parking.Data.Entites
{
    public class Off25Coupon:Coupon
    {
        public Off25Coupon()
        {
            CouponType = CouponType.Off25;
        }
        public override int GetDiscount(int cost)
        {
            if(IsUsed || !IsActive())
                throw new InvalidOperationException("Coupon was used or not active");
            return (int)(cost*0.75);
        }

        public override Coupon CreateInstance(Coupon coupon)
        {
            if(coupon.CouponType == CouponType.Off25)
            {
                return new Off25Coupon()
                {
                    Token = coupon.Token,
                    IsUsed = coupon.IsUsed,
                    Start = coupon.Start,
                    End = coupon.End,
                    Name = coupon.Name,
                    Id = coupon.Id
                };
            }
            else throw new ArgumentException("Coupon type is Invalid!");
        }
    }
}