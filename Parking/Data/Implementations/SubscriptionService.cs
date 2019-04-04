using System;
using Parking.Data.Factories;
using Parking.Data.Api;
using Parking.Data.Entites;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Data.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        ISubscriptionDataContext _context;

        public SubscriptionService([FromServices]ISubscriptionDataContext context)
        {
            _context = context;
        }
        public async Task<Subscription> FindSubscription(string userEmail)
        {
            var s = await FindUserSubscription(userEmail);
            if(s == null)
            throw new ArgumentException("Not exist user email");

            if(s.Expired > DateTime.Now)
                throw new ArgumentException("Subscription is expired");

            return s.Subscription;
        }

        private Task<UserSubscription> FindUserSubscription(string userEmail)
        {
            return _context.UserSubscriptions.FirstOrDefaultAsync(x => x.User.NormalizedEmail == userEmail.ToUpper());
        }
    }
}