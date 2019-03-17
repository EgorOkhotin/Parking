using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Parking.Data
{
    public class DataProperties : IDataProperties
    {
        IConfiguration configuration;
        public DataProperties([FromServices] IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetDefaultTariffName()
        {
            return configuration.GetValue<string>("DefaultTariff");
        }
    }
}