using System;
using System.Threading.Tasks;
using Parking.Models;
using Parking.Services.Api;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parking.Data.Entites;

namespace Parking.Services.Implementations
{
    public class EnterService : IEnterService
    {
        IKeyDataService _keyService;
        ILogger<IEnterService> _logger;
        IEntityFactory _entityFactory;
        IModelCreateService _modelCreator;
        ICostCalculation _costCalculationService;
        IStatisticService _statistic;
        public EnterService([FromServices] IKeyDataService keyService,
        [FromServices] IEntityFactory entityFactory,
        [FromServices] IModelCreateService modelsCreator,
        [FromServices] ICostCalculation costCalculationService,
        [FromServices] IStatisticService statistic,
        [FromServices] ILogger<IEnterService> logger)
        {
            _keyService = keyService;
            _logger = logger;
            _entityFactory = entityFactory;
            _modelCreator = modelsCreator;
            _costCalculationService = costCalculationService;
            _statistic = statistic;
        }

        public Task<Models.Key> EnterForAnonymous(string tariffName = null)
        {
            return GetKey(tariffName);
        }

        public Task<Models.Key> EnterForAuthorize(string tariffName = null)
        {
            return GetKey(tariffName);
        }

        public Task<Models.Key> EnterForAuthorizeByAutoId(string autoId, string tariffName = null)
        {
            if(autoId==null)
            {
                _logger.LogWarning($"{GetType().Name}: Auto id was null!");
                throw new ArgumentNullException();
            } 
            return GetKey(tariffName, autoId);
        }

        public async Task<int> GetCost(string autoId, string token, string userEmail = null, string coupon = null)
        {
            if(autoId== null && token== null)
                throw new ArgumentNullException();
            
            var k = await FindKey(token, autoId);
            return _costCalculationService.GetCost(k, userEmail, coupon);
        }

        //TODO add sum check
        public async Task<bool> Leave(string token, int cost)
        {
            if(token == null || cost<0) throw new ArgumentException();
            var result = await _keyService.Delete(token);
            // if(result)
            //     _statistic.

            return result;
        }

        private async Task<Models.Key> GetKey(string tariffName, string autoId = null)
        {
            var k = _entityFactory.CreateKey(tariffName, autoId);
            
            bool isAdd = await _keyService.Add(k);
            if(!isAdd) throw new SystemException($"Can't add key!");
            
            return _modelCreator.CreateKeyModel(k);
        }

        private async Task<Data.Entites.Key> FindKey(string token, string autoId)
        {
            Data.Entites.Key k;
            if(autoId==null)
                k = await _keyService.GetByToken(token);
            else
                k = await _keyService.GetByAutoId(autoId);

            return k;
        }
    }
}