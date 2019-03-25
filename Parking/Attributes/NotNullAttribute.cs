using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Parking.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class NotNullAttribute : ParameterAttribute
    {
        public NotNullAttribute()
        {
            
        }
        public override bool IsValid(object value)
        {
            if(value != null)
            {
                return true;
            }
            return false;
        }
    }
}