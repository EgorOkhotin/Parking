using System;

namespace Parking.Data.Entites
{
    public class UserSubscription
    {
        public int Id{get;set;}
        public ApplicationUser User{get;set;}
        public Subscription Subscription{get;set;}
        public DateTime Expired{get;set;}
    }
}