using System;
using System.Collections.Generic;

namespace Parking.Data.Entites
{
    public class Subscription : Discount
    {
        public string TariffNames{get;set;}

        public List<UserSubscription> Subscriptions {get;set;}

        public override int GetDiscount(int cost)
        {
            return 0;
        }
    }
}