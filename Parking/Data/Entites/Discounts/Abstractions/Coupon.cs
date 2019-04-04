using System;

namespace Parking.Data.Entites
{
    public class Coupon : Discount
    {
        public string Token{get;set;}
        public bool IsUsed{get;set;}

        public CouponType CouponType {get;set;}

        public override bool IsActive()
        {
            return (!IsUsed) && base.IsActive();
        }

        public override int GetDiscount(int cost)
        {
            throw new NotImplementedException();
        }

        public virtual Coupon CreateInstance(Coupon coupon)
        {
            throw new NotImplementedException();
        }
    }
}