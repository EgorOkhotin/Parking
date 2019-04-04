using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface ISubscriptionService
    {
        Task<Subscription> FindSubscription(string userEmail);
        
    }
}