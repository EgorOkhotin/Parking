using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface ISellOutService
    {
        Task<SellOut> FindSellOut(string tariffName);
    }
}