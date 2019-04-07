using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface ISubscriptionDataService
    {
        Task<Subscription> FindSubscription(string userEmail);
        
    }
}