using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Models
{
    public class Key
    {
        const string TOKEN_DEFAULT_STRING = "EMPTY_TOKEN";
        const string TARIFF_DEFAULT_STRING = "EMPTY_TARIFF";
        const string AUTOID_DEFAULT_STRING = "EMPTY_AUTOID";
        public string Token { get; set; }

        public string AutoId{get;set;}
        public DateTime TimeStamp { get; set; }
        public Tariff Tariff { get; set; }

        public override string ToString()
        {
            return $"|KEY| Token:{Token??TOKEN_DEFAULT_STRING}\n\t Auto ID:{AutoId??AUTOID_DEFAULT_STRING} \n\t Time:{TimeStamp} \n\t {TariffToString()}";
        }

        private string TariffToString()
        {
            return Tariff == null ? TARIFF_DEFAULT_STRING : Tariff.ToString();
        }
    }
}
