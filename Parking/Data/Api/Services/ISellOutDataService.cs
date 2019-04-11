using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api.Services
{
    public interface ISellOutDataService
    {
        Task<SellOut> FindSellOut(string tariffName);
        Task<bool> UseSellOut(SellOut sellOut);
    }
}