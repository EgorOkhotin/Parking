using System;
using Parking.Data.Entites;
using System.Threading.Tasks;

namespace Parking.Data.Api.Services
{
    public interface ITariffDataService
    {
        Task<Tariff> GetByName(string name);
        Task<Tariff> GetById(int id);
        Task<bool> Add(Tariff tariff);
        Task<bool> Remove(string name);
    }
}