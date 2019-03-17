using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Models;
using System;
using System.Collections.Generic;
using Parking.Data;
using System.Linq;
using System.Threading.Tasks;
using Parking.Services.Api;

namespace Parking.Controllers
{
    public class ParkingController : Controller
    {
        IDataAdapter data;
        ICostCalculation calculation;
        public ParkingController([FromServices] IDataAdapter adapter, 
            [FromServices] ICostCalculation calculator)
        {
            this.data = adapter;
            this.calculation = calculator;
        }
        [AllowAnonymous]
        public async Task<Models.Key> Enter(string tariffName)
        {
            return await data.CreateKey(tariffName);
        }

        [Authorize(Roles="user")]
        public async Task<Models.Key> EnterUser(string tariffName)
        {
            return await data.CreateKey(tariffName);
        }

        [Authorize(Roles = "employee")]
        public async Task<Models.Key> EnterEmployee(string tariffName)
        {
            return await data.CreateKey(tariffName);
        }

        public async Task<int> GetCost(string token)
        {
            var k = await data.FindKey(token);
            return calculation.GetCost(k.TimeStamp, k.Tariff.Cost);
        }

        //TODO Add sum check
        public async Task<bool> GetPay(string token, int cost)
        {
            
            return await data.DeleteKey(new Models.Key() { Token = token });
        }
    } 
}
