using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data
{
    public interface IKeyFactory
    {
        Key CreateKey(Tariff tariff);
        Key CreateKey(Tariff tariff, string autoId);
    }
}
