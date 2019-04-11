using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api.Services
{
    public interface ISubscriptionDataService
    {
        Task<Subscription> FindSubscription(string userEmail);
        
    }
}