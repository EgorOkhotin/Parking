using System;
using Parking.Services.Api;
using System.Reflection;
using System.Diagnostics;
using Parking.Attributes;
using System.Linq;

namespace Parking.Services.Implementations
{
    public class ValidationService : IValidationService
    {
        public bool IsValid()
        {
            // var stack = new StackTrace();
            // var invokedMethod = stack.GetFrame(1).GetMethod();
            // // var param = stack.GetFrame(1).GetMethod().
            // foreach(var p in invokedMethod.GetParameters())
            // {
            //     var t = p.GetType();
            //     if(IsDefined(p))
            //     {
            //         var attributes = GetParameterAttributes(p);
            //         foreach(var a in attributes)
            //         {
            //             var attribute = (ParameterAttribute)a;
            //             attribute.IsValid(p.)
            //         }

            //     }
            // }
            return true;
        }

        private bool IsDefined(ParameterInfo info)
        {
            return GetParameterAttributes(info).Length>0;
        }

        private object[] GetParameterAttributes(ParameterInfo info)
        {
            return info.GetCustomAttributes(typeof(ParameterAttribute), true);
        }
    }
}