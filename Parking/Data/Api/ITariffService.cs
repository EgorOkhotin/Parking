using System;
using Parking.Data;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface ITariffService
    {
        Task<Tariff> GetByName(string name);
        Task<Tariff> GetById(int id);
        Task<bool> Add(Tariff tariff);
        Task<bool> Remove(string name);
    }
}