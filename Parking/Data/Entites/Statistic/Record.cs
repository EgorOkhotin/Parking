using System;
using System.ComponentModel.DataAnnotations;

namespace  Parking.Data.Entites.Statistic
{
    public class Record
    {
        [Key]
        public DateTime Time{get;set;}
        public string Description {get;set;}
    }
}