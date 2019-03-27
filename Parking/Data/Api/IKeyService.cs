using Parking.Data;
using System.Threading.Tasks;
namespace Parking.Data.Api
{
    public interface IKeyService
    {
        Task<Key> GetByToken(string token);
        Task<Key> GetByAutoId(string autoId);
        Task<bool> Add(Key key);
        Task<bool> Delete(string token);
    }
}