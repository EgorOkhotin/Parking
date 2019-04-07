using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Data.Api;
using Parking.Data.Entites;


namespace Parking.Data
{
    public class KeyFactory : IKeyFactory
    {
        public Key CreateKey(Tariff tariff)
        {
            return CreateKey(tariff,null);
        }

        public Key CreateKey(Tariff tariff, string autoId)
        {
            var k = new Key()
            {
                Tariff = tariff,
                TariffId = tariff.Id,
                AutoId = autoId,
                TimeStamp = GetStamp(),
                Token = GetToken()
            };
            tariff.Keys.Add(k);
            return k;
        }

        private DateTime GetStamp()
        {
            return DateTime.Now; 
        }

        private string GetToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
