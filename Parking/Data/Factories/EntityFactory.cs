using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Data.Api.Factories;
using Parking.Data.Api.Services;
using Parking.Data.Api;
using Parking.Data.Entites;

namespace Parking.Data
{
    class EntityFactory : IEntityFactory
    {
        IDataProperties _properties;
        ITariffDataService _tariffs;
        IKeyFactory _keyFactory;
        public EntityFactory([FromServices] IDataProperties dataProperties,
        [FromServices] ITariffDataService tariffs,
        [FromServices] IKeyFactory keyFactory)
        {
            _properties = dataProperties;
            _tariffs = tariffs;
            _keyFactory = keyFactory;
        }

        public Key CreateKey(string tariffName)
        {
            return CreateKey(tariffName, null);
        }

        public Key CreateKey(string tariffName, string autoId)
        {
            if(tariffName == null)
                tariffName = _properties.GetDefaultTariffName();

            var tariff = GetTariff(tariffName).Result;
            if(tariff == null) throw new ArgumentException("Tariff don't exist");

            if(autoId == null)
                return _keyFactory.CreateKey(tariff);
            
            return _keyFactory.CreateKey(tariff, autoId);
        }



        private async Task<Tariff> GetTariff(string name)
        {
            var t = await _tariffs.GetByName(name);
            return t;
        }
    }
}