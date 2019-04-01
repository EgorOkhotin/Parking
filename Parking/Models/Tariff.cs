using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Models
{
    public class Tariff
    {
        const string DEFAULT_NAME = "EMPTY_NAME";
        public string Name { get; set; }
        public int Cost { get; set; }

        public override string ToString()
        {
            return $"|TARIFF| Name:{Name??DEFAULT_NAME}\t Cost per period:{Cost}";
        }
    }
}
