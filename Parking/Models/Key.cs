using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Models
{
    public class Key
    {
        public string Token { get; set; }
        public DateTime TimeStamp { get; set; }
        public Tariff Tariff { get; set; }
    }
}
