using System;
using System.Threading.Tasks;

namespace Parking.Services.Api
{
    public interface IDiscountService
    {
        Task<int> GetCost(int cost, string tariffName, string userEmail = null, string coupon = null);
    }
}