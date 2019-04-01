using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data.Entites
{
    public class Key
    {
        //FOR ToString method
        const string TOKEN_DEFAULT_STRING = "EMPTY_TOKEN";
        const string TARIFF_DEFAULT_STRING = "EMPTY_TARIFF";
        const string AUTOID_DEFAULT_STRING = "EMPTY_AUTOID";
        public Key()
        {
            Token = new Guid().ToString();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public Tariff Tariff { get; set; }

        [Required]
        public int? TariffId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public string AutoId {get;set;}

        public override string ToString()
        {
            return $"|KEY| Token:{Token??TOKEN_DEFAULT_STRING} \n\t Auto ID:{AutoId??AUTOID_DEFAULT_STRING} \n\t TimeStamp:{TimeStamp} \n\t Tariff:{TariffToString()} \n";
        }

        private string TariffToString()
        {
            return Tariff == null ? TARIFF_DEFAULT_STRING : Tariff.ToString();
        }
    }
}
