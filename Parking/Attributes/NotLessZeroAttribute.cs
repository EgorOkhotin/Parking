using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Parking.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class NotLessZeroAttribute : ParameterAttribute
    {
        public NotLessZeroAttribute()
        {

        }
        public override bool IsValid(object value)
        {
            if(value == null) throw new ArgumentNullException();
            if(IsDigit(value))
            {
                int v = Convert.ToInt32(value);
                if(v>0) return true;
            }
            return false;
            // throw new ArgumentException($"Invalid parametr! {value}");
        }

        private bool IsDigit(object obj)
        {
            try
            {
                int res = Convert.ToInt32(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}