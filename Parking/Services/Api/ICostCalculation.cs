using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Data.Entites;

namespace Parking.Services.Api
{
    public interface ICostCalculation
    {
        int GetCost(Key key);
        //int GetCostWithDiscount(Key key, )
        int GetCost(DateTime timeStamp, int cost);
    }
}
