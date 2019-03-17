using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Parking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data
{
    public class DataAdapter : IDataAdapter
    {
        IDatabaseContext database;
        IDataProperties properties;
        IKeyFactory keyFactory;
        
        public DataAdapter([FromServices] IDatabaseContext context, 
            [FromServices] IDataProperties properties,
            [FromServices] IKeyFactory keyFactory)
        {
            database = context;
            this.properties = properties;
            this.keyFactory = keyFactory;
        }

        public Task<Models.Key> CreateKey()
        {
            return CreateKey(properties.GetDefaultTariffName());
        }

        public async Task<Models.Key> CreateKey(string tariffName)
        {
            var tariff = await database.GetTariff(tariffName);
            var k = keyFactory.CreateKey(tariff);

            await database.AddKey(k);
            var tariffModel = new Models.Tariff() { Name = tariff.Name, Cost = tariff.Cost };
            var result = new Models.Key() { Token = k.Token, TimeStamp = k.TimeStamp, Tariff = tariffModel };

            return result;
        }

        public async Task<Models.Key> FindKey(string token)
        {
            var k = await database.GetKey(token);
            var result = new Models.Key()
            {
                Token = token,
                TimeStamp = k.TimeStamp,
                Tariff = new Models.Tariff()
                {
                    Name = k.Tariff.Name,
                    Cost = k.Tariff.Cost
                }
            };
            return result;
        }

        public async Task<bool> DeleteKey(Models.Key key)
        {
            return await database.DeleteKey(await database.GetKey(key.Token));
        }

        public async Task<Models.Tariff> GetTariff(string tariffName)
        {
            var t = await database.GetTariff(tariffName);
            return new Models.Tariff() { Cost = t.Cost, Name = tariffName };
        }
    }
}
