using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Parking.Data.Entites;
using Parking.Data.Factories.Abstractions;

namespace Parking.Data.Factories
{
    public class DefaultCouponFactory : CouponFactory
    {
        Coupon[] _coupons;
        public DefaultCouponFactory()
        {
            _coupons = new Coupon[3];
            _coupons[0] = new NoneCoupon();
            _coupons[1] = new Off50Coupon();
            _coupons[2] = new Off25Coupon();
        }
        public override Coupon GetCoupon(Coupon coupon)
        {
            var t = (int)coupon.CouponType;
            if(!((t>=0) && (t<_coupons.Length)))
                throw new ArgumentException("Incorrect coupon type!");
            
            return _coupons[t].CreateInstance(coupon);
        }
    }
}