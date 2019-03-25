using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data
{
    public interface IDatabaseContext
    {
        Task<bool> AddKey(Key key);
        Task<bool> DeleteKey(Key key);
        Task<Key> GetKey(string token);
        Task<Key> GetKeyByAutoId(string autoId);
        Task<Tariff> GetTariff(string tariffName);
    }
}
