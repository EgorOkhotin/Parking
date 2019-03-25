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
using System.Security.Claims;

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

        public async Task<Models.Key> Enter(string tariffName, string autoId = null)
        {
            if(tariffName == null) return BadRequest<Models.Key>("Tariff name is not correct!");
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                if(HttpContext.User.IsInRole("user"))
                {
                    //TODO: hide in separate method
                    //TODO: add tariff validation
                    _logger.LogInformation($"Enter on parking: {tariffName} \t by user: {HttpContext.User.Identity.Name}");
                    return await CreateKey(tariffName, autoId);
                }
                else if(HttpContext.User.IsInRole("employee"))
                {
                    //TODO: hide in separate method
                    //TODO: add tariff validation
                    _logger.LogInformation($"Enter on parking: {tariffName} \t by employee: {HttpContext.User.Identity.Name}");
                    return await CreateKey(tariffName, autoId);
                }
            }
            _logger.LogInformation($"Created key for: {tariffName}");
            return await CreateKey(tariffName, autoId);
        }

        public async Task<int?> GetCost(string autoId, string token = null)
        {
            if(autoId == null && token == null)
                return BadRequest<int?>("Incorrect parametrs!");

            if(autoId != null)
            {
                _logger.LogInformation($"Try to get cost by auto id: {autoId}");
                return await GetCostByAutoId(autoId);
            }
            
            _logger.LogInformation($"Try to get cost by token: {token}");
            return await GetCostByToken(token);
        }

        private async Task<int?> GetCostByToken(string token)
        {
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

        private async Task<int?> GetCostByAutoId(string autoId)
        {
            var k = await _data.FindKeyByAutoId(autoId);
            if (k != null)
            {
                _logger.LogInformation($"Founded key for autoId: {autoId} is \n\t {k}");

                var result = _calculation.GetCost(k.TimeStamp, k.Tariff.Cost);
                _logger.LogInformation($"Calculated cost for autoId: {autoId} \t is {result}");
                return result;
            }

            _logger.LogError($"Key by token:{autoId} didn't find!");
            return BadRequest<int?>("AutoId is not correct!");
        }

        //TODO Add sum check
        public async Task<bool> GetPay(string token, int cost)
        {
            if(token != null && cost>=0){
                _logger.LogInformation($"Get pay: {cost} \t by token: {token}");
                return await _data.DeleteKey(new Models.Key() { Token = token });
            }
            return false;
        }

        //TODO Review bad request conception
        private T BadRequest<T>(string message)
        {
            return BadRequest(message, default(T));
        }
        private T BadRequest<T>(string message, T t)
        {
            HttpContext.Response.StatusCode = 400;
            HttpContext.Response.Body =
                new MemoryStream(
                    Encoding.Unicode.GetBytes(message));
            return t;
        }

        private async Task<Models.Key> CreateKey(string tariffName)
        {
            return await CreateKey(tariffName, null);
        }

        private async Task<Models.Key> CreateKey(string tariffName, string autoId)
        {
            var k = await _data.CreateKey(tariffName,autoId);
            _logger.LogInformation($"Created key token:{k.Token}");
            if (k != null) return k;
            return BadRequest<Models.Key>($"Tariff name({tariffName}) is wrong");
        }

        private async Task<Models.Key> EnterUser(string tariffName, string autoId, ClaimsPrincipal user)
        {
            if(await IsValidTariff(tariffName, user))
            {
                return await CreateKey(tariffName, autoId);
            }
            _logger.LogWarning($"Invalid tariff for user: {user.Identity.Name}. Requested tariff: {tariffName}");
            return null;
        }

        private async Task<bool> IsValidTariff(string tariffName, ClaimsPrincipal user)
        {
            return await Task.Run(()=>{
                return true;
            });
        }
    }
}
