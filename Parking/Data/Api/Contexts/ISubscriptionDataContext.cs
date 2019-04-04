using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data.Entites;
using System.Threading;

namespace Parking.Data.Api
{
    public interface ISubscriptionDataContext : IDataContext
    {
        DbSet<Subscription> Subscriptions {get;set;}
        DbSet<UserSubscription> UserSubscriptions {get;set;}
        DbSet<ApplicationUser> Users {get;set;}
    }
}