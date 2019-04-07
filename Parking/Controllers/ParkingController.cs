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
        IEnterService _enterService;
        ICostCalculation _calculation;
        private readonly ILogger _logger;
        public ParkingController([FromServices] IEnterService enterService,
            [FromServices] ICostCalculation calculator,
            [FromServices] ILogger<ParkingController> logger) : base()
        {
            this._enterService = enterService;
            this._calculation = calculator;
            this._logger = logger;
        }

        public async Task<Models.Key> Enter(string tariffName)
        {
            if (tariffName == null)
                return BadRequest<Models.Key>("Tariff name was null");
            try
            {
                return await _enterService.EnterForAnonymous(tariffName);
            }
            catch (ArgumentException)
            {
                _logger.LogError($"{GetType().Name}: Can't enter on parking with tariff name {tariffName}");
                return BadRequest<Models.Key>("");
            }
            catch (SystemException sysException)
            {
                _logger.LogError($"{GetType().Name} Can't enter on parking with tariff name {tariffName} {sysException.Message}");
                return BadRequest<Models.Key>("");
            }
        }

        [Authorize]
        public async Task<Models.Key> EnterUser(string tariffName)
        {
            if (tariffName == null)
                return BadRequest<Models.Key>("Tariff name was null");
            try
            {
                return await _enterService.EnterForAuthorize(tariffName);
            }
            catch (ArgumentException)
            {
                _logger.LogError($"{GetType().Name}: Can't enter on parking with tariff name {tariffName} \n\t By user:{HttpContext.User.Identity.Name}");
                return BadRequest<Models.Key>("");
            }
            catch (SystemException sysException)
            {
                _logger.LogError($"{GetType().Name} Can't enter on parking with tariff name {tariffName} {sysException.Message} \n\t \n\t By user:{HttpContext.User.Identity.Name}");
                return BadRequest<Models.Key>("");
            }
        }

        [Authorize]
        public async Task<Models.Key> EnterUser(string tariffName, string autoId)
        {
            if (tariffName == null)
                return BadRequest<Models.Key>("Tariff name was null");
            try
            {
                return await _enterService.EnterForAuthorizeByAutoId(tariffName, autoId);
            }
            catch (ArgumentException argException)
            {
                _logger.LogError($"{GetType().Name}: Can't enter on parking with tariff name {tariffName} \n\t By user:{HttpContext.User.Identity.Name} \n\t {argException.Message}");
                return BadRequest<Models.Key>("");
            }
            catch (SystemException sysException)
            {
                _logger.LogError($"{GetType().Name} Can't enter on parking with tariff name {tariffName} {sysException.Message} \n\t \n\t By user:{HttpContext.User.Identity.Name}");
                return BadRequest<Models.Key>("");
            }
        }

        public async Task<int?> GetCost(string autoId, string token = null, string coupon = null)
        {
            try
            {
                string userEmail = null;
                if(HttpContext.User.Identity.IsAuthenticated)
                    userEmail = HttpContext.User.Identity.Name;
                return await _enterService.GetCost(autoId, token, userEmail, coupon);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"{GetType().Name}: Get cost failed! {ex.Message}");
                return BadRequest<int?>("");
            }
        }

        //TODO Add sum check
        public async Task<bool> GetPay(string token, int cost)
        {
            try
            {
                return await _enterService.Leave(token, cost);
            }
            catch(ArgumentException)
            {
                _logger.LogError($"{GetType().Name}: Cant get pay for {token}; Cost is {cost}");
                return false;
            }
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
    }
}
