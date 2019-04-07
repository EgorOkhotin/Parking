using Parking.Data.Entites;
using System.Threading.Tasks;
namespace Parking.Data.Api
{
    public interface IKeyDataService
    {
        Task<Key> GetByToken(string token);
        Task<Key> GetByAutoId(string autoId);
        Task<bool> Add(Key key);
        Task<bool> Delete(string token);
    }
}