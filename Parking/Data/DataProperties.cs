using System;
using Parking.Data.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Parking.Data
{
    public class DataProperties : IDataProperties
    {
        IConfiguration _configuration;
        public DataProperties([FromServices] IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetDefaultTariffName()
        {
            return _configuration.GetValue<string>("DefaultTariff");
        }
    }
}