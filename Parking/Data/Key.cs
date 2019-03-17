using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Parking.Data
{
    public class Key
    {
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
    }
}
