using System;
using Parking.Data.Entites;

namespace Parking.Data.Api
{
    public interface IParkingDataService
    {
        Data.Entites.Parking GetPlace(string name);

    }
}