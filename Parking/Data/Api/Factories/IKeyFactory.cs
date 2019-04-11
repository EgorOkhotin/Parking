using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Data.Entites;

namespace Parking.Data.Api.Factories
{
    public interface IKeyFactory
    {
        Key CreateKey(Tariff tariff);
        Key CreateKey(Tariff tariff, string autoId);
    }
}
