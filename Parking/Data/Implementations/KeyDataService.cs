using System;
using Parking.Data;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;

namespace Parking.Data.Implementations
{
    public class KeyDataService : IKeyService
    {
        ILogger<IKeyService> _logger;
        IKeyDataContext _context;
        public KeyDataService([FromServices]IKeyDataContext context,
        [FromServices] ILogger<IKeyService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> Add(Key key)
        {
            try
            {
                _context.Keys.Add(key);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"{GetType().Name}: Key did not add! {ex.Message}");
            }
            return false;
        }

        public async Task<bool> Delete(string token)
        {
            var key = await _context.Keys.FirstOrDefaultAsync(x => x.Token == token);
                if(key != null)
                {
                    _context.Keys.Remove(key);
                    return true;
                }
                _logger.LogWarning($"{GetType().Name}: Key token didn't exist: {token}");
                return false;
        }

        public async Task<Key> GetByAutoId(string autoId)
        {
            var k = await _context.Keys.FirstOrDefaultAsync(x => x.AutoId == autoId);
            if(k == null)
                _logger.LogWarning($"{GetType().Name}: Key didn't find by auto id:{autoId}");
            return k;
        }

        public async Task<Key> GetByToken(string token)
        {
            var k = await _context.Keys.FirstOrDefaultAsync(x => x.Token == token);
            if(k == null)
                _logger.LogWarning($"{GetType().Name}: Key didn't find by token:{token}");
            return k;
        }
    }
}