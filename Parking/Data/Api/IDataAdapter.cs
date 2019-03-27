using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data.Api
{
    public interface IDataAdapter
    {
        Task<Models.Key> CreateKey();
        Task<Models.Key> CreateKey(string tariffName);
        Task<Models.Key> CreateKey(string tariffName, string autoId);
        Task<Models.Key> FindKey(string token);
        Task<Models.Key> FindKeyByAutoId(string autoId);
        Task<Models.Tariff> GetTariff(string tariffName);
        Task<bool> DeleteKey(Models.Key key);
    }
}
