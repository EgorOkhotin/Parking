using System;
using Microsoft.AspNetCore.Identity;

namespace Parking.Data.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public int? UserSubscriptionId{get;set;}
        public UserSubscription UserSubscription{get;set;}

        
    }
}