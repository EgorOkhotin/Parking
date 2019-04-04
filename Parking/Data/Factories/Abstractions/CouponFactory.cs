using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Parking.Data.Entites;

namespace Parking.Data.Factories.Abstractions
{
    public abstract class CouponFactory : ICouponFactory
    {
        public abstract Coupon GetCoupon(Coupon coupon);
    }
}