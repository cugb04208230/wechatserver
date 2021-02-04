using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Encoo.LowCode.WechatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WechatController : ControllerBase
    {
        private readonly IWechatApi _wechatApi;
        private readonly WechatServerDataContext _dbContext;
        private readonly CacheService _cacheService;
        public WechatController(IWechatApi wechatApi, WechatServerDataContext dbContext, CacheService cacheService)
        {
            this._wechatApi = wechatApi;
            this._dbContext = dbContext;
            this._dbContext.WechatSessionInfos.Add(new WechatSessionInfo { Corpid = Guid.NewGuid().ToString(), Userid = Guid.NewGuid().ToString(), SessionKey = Guid.NewGuid().ToString() });
            this._cacheService = cacheService;
        }

        [HttpGet]
        public IActionResult GetCacheValue(string type)
        {
            switch (type)
            {
                case "SuiteTicket":
                    return Ok(_cacheService.SuiteTicket);
                case "PermanentCodes":
                    return Ok(_cacheService.PermanentCodes);
                case "Departments":
                    return Ok(_cacheService.Departments);
                case "DepartmentUsers":
                    return Ok(_cacheService.DepartmentUsers);
            }
            return Ok("Can not find");
        }

        [HttpGet("ticket")]
        public IActionResult GetTicket(string ticket)
        {
            if (_cacheService.Tickets.ContainsKey(ticket))
            {
                return Ok(_cacheService.Tickets[ticket]);
            }
            return Ok("Can not find");
        }
    }
}
