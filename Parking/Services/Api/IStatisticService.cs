using System;
using Parking.Data.Api;
using Parking.Data.Entites;

namespace Parking.Services.Api
{
    public interface IStatisticService
    {
        bool EnterUser(string name, string tariff);
        bool LeaveUser(string name, string tariff);
    }
}