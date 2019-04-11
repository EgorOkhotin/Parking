using System;
using Parking.Models;
using Parking.Data;
using System.Threading.Tasks;
namespace Parking.Services.Api
{
    public interface IEnterService
    {
        Task<Models.Key> EnterForAnonymous(string tariffName);

        Task<Models.Key> EnterForAuthorize(string tariffName);
        Task<Models.Key> EnterForAuthorizeByAutoId(string tariffName, string autoId);

        Task<int> GetCost(string autoId, string token, string userEmail = null, string coupon = null);
    }
}