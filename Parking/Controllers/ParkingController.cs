using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Models;
using System;
using System.Collections.Generic;
using Parking.Data;
using System.Linq;
using System.Threading.Tasks;
using Parking.Services.Api;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using Parking.Attributes;

namespace Parking.Controllers
{
    public class ParkingController : Controller
    {
        IDataAdapter _data;
        ICostCalculation _calculation;
        private readonly ILogger _logger;
        public ParkingController([FromServices] IDataAdapter adapter,
            [FromServices] ICostCalculation calculator,
            [FromServices] ILogger<ParkingController> logger) : base()
        {
            this._data = adapter;
            this._calculation = calculator;
            this._logger = logger;
        }

        [AllowAnonymous]
        public async Task<Models.Key> Enter([NotNull] string tariffName)
        {
            if(tariffName == null) return BadRequest<Models.Key>("Tariff name is not correct!");
            _logger.LogInformation($"Enter on parking: {tariffName}");

            return await CreateKey(tariffName);
        }

        public async Task<Models.Key> EnterByAutoId(string tariffName, string autoId)
        {
            if(tariffName == null) return BadRequest<Models.Key>("Tariff name is not correct!");
            if(autoId == null) return BadRequest<Models.Key>("Auto id is not correct!");
            _logger.LogInformation($"Enter on parking: {autoId}");
            return await CreateKey(tariffName, autoId);
        }

        [Authorize(Roles = "user")]
        public async Task<Models.Key> EnterUser([NotNull] string tariffName)
        {
            if(tariffName == null) return BadRequest<Models.Key>("Tariff name is not correct!");
            // _logger.LogInformation($"Enter on parking: {tariffName} \t by user: {HttpContext.User.Identity.Name}");

            return await CreateKey(tariffName);
        }

        [Authorize(Roles = "employee")]
        public async Task<Models.Key> EnterEmployee([NotNull] string tariffName)
        {
            if(tariffName == null) return BadRequest<Models.Key>("Tariff name is not correct!");
            // _logger.LogInformation($"Enter on parking: {tariffName} \t by employee: {HttpContext.User.Identity.Name}");

            return await CreateKey(tariffName);
        }

        public async Task<int?> GetCost([NotNull] string token)
        {
            if(token == null) return BadRequest<int?>("Token is not correct!");
            _logger.LogInformation($"Try to get cost by token: {token}");

            _logger.LogInformation($"Try to find key for token: {token}");
            var k = await _data.FindKey(token);
            if (k != null)
            {
                _logger.LogInformation($"Founded key for token: {token} \t is {k}");

                var result = _calculation.GetCost(k.TimeStamp, k.Tariff.Cost);
                _logger.LogInformation($"Calculated cost for token: {token} \t is {result}");
                return result;
            }

            _logger.LogError($"Key by token:{token} didn't find!");
            return BadRequest<int?>("Token is not correct!");
        }

        public async Task<int?> GetCostByAutoId([NotNull] string autoId)
        {
            if(autoId == null) return BadRequest<int?>("AutoId is not correct!");
            _logger.LogInformation($"Try to get cost by autoId: {autoId}");

            _logger.LogInformation($"Try to find key for autoId: {autoId}");
            var k = await _data.FindKeyByAutoId(autoId);
            if (k != null)
            {
                _logger.LogInformation($"Founded key for autoId: {autoId} \t is {k}");

                var result = _calculation.GetCost(k.TimeStamp, k.Tariff.Cost);
                _logger.LogInformation($"Calculated cost for autoId: {autoId} \t is {result}");
                return result;
            }

            _logger.LogError($"Key by token:{autoId} didn't find!");
            return BadRequest<int?>("AutoId is not correct!");
        }

        //TODO Add sum check
        public async Task<bool> GetPay([NotNull] string token, [NotLessZero]int cost)
        {
            if(token != null && cost>=0){
                _logger.LogInformation($"Get pay: {cost} \t by token: {token}");
                return await _data.DeleteKey(new Models.Key() { Token = token });
            }
            return false;
        }

        private T BadRequest<T>(string message)
        {
            return BadRequest(message, default(T));
        }
        //TODO Review untesting code
        private T BadRequest<T>(string message, T t)
        {
            // HttpContext.Response.StatusCode = 400;
            // HttpContext.Response.Body =
            //     new MemoryStream(
            //         Encoding.Unicode.GetBytes(message));
            return t;
        }

        private async Task<Models.Key> CreateKey(string tariffName)
        {
            return await CreateKey(tariffName, null);
        }

        private async Task<Models.Key> CreateKey(string tariffName, string autoId)
        {
            var k = await _data.CreateKey(tariffName,autoId);
            _logger.LogInformation($"Created key:{k}");
            if (k != null) return k;
            return BadRequest<Models.Key>($"Tariff name({tariffName}) is wrong");
        }


    }
}
