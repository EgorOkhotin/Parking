using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Services.Api
{
    public interface ICostCalculation
    {
        int GetCost(DateTime timeStamp, int cost);
    }
}
