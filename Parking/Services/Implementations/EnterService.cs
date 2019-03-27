using System;
using System.Threading.Tasks;
using Parking.Models;
using Parking.Services.Api;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Parking.Services.Implementations
{
    public class EnterService : IEnterService
    {
        IKeyService _keyService;
        ITariffService _tariffService;
        ILogger<IEnterService> _logger;
        IDataProperties _dataProperties;
        IKeyFactory _keyFactory;
        ICostCalculation _costCalculationService;
        public EnterService([FromServices] IKeyService keyService,
        [FromServices] ITariffService tariffService,
        [FromServices] IDataProperties dataProperties,
        [FromServices] IKeyFactory keyFactory,
        [FromServices] ICostCalculation costCalculationService,
        [FromServices] ILogger<IEnterService> logger)
        {
            _keyService = keyService;
            _tariffService = tariffService;
            _logger = logger;
            _dataProperties = dataProperties;
            _keyFactory = keyFactory;
            _costCalculationService = costCalculationService;
        }

        public Task<Key> EnterForAnonymous(string tariffName = null)
        {
            return GetKey(tariffName);
        }

        public Task<Key> EnterForAuthorize(string tariffName = null)
        {
            return GetKey(tariffName);
        }

        public Task<Key> EnterForAuthorizeByAutoId(string autoId, string tariffName = null)
        {
            return GetKey(tariffName, autoId);
        }

        public async Task<int> GetCost(string autoId, string token)
        {
            if(autoId== null && token== null)
                throw new ArgumentNullException();
            
            var k = await FindKey(token, autoId);
            return _costCalculationService.GetCost(k.TimeStamp, k.Tariff.Cost);
        }

        //TODO add sum check
        public Task<bool> Leave(string token, int cost)
        {
            if(token == null || cost<0) throw new ArgumentException();
            return _keyService.Delete(token);
        }

        private async Task<Models.Key> GetKey(string tariffName, string autoId = null)
        {
            if (tariffName == null)
                tariffName = _dataProperties.GetDefaultTariffName();

            var t = await GetTariff(tariffName);
            var k = await CreateNewKey(t, autoId);

            var isAdd = await _keyService.Add(k);
            if(!isAdd) throw new SystemException($"Can't add key!");

            return new Models.Key(){
                Token = k.Token,
                TimeStamp = k.TimeStamp,
                Tariff = new Tariff(){
                    Name = t.Name,
                    Cost = t.Cost,
                }
            };
        }

        private async Task<Data.Tariff> GetTariff(string tariffName)
        {
            var t = await _tariffService.GetByName(tariffName);
            if (t == null)
            {
                _logger.LogError($"{GetType().Name}: Tariff with name:{tariffName} didn't find!");
                throw new ArgumentException(nameof(tariffName));
            }
            return t;
        }

        private async Task<Data.Key> CreateNewKey(Data.Tariff tariff, string autoId)
        {
            return await Task.Run(() =>{
                Data.Key k;
                if (autoId != null)
                    k = _keyFactory.CreateKey(tariff, autoId);
                else
                    k = _keyFactory.CreateKey(tariff);
                return k;
            });
        }

        private async Task<Data.Key> FindKey(string token, string autoId)
        {
            Data.Key k;
            Data.Tariff t;
            if(autoId==null)
                k = await _keyService.GetByToken(token);
            else
                k = await _keyService.GetByAutoId(autoId);

            t = await _tariffService.GetById(k.TariffId.Value);
            
            k.Tariff = t;
            return k;
        }
    }
}