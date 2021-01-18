using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Models;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WechatController : ControllerBase
    {
        private readonly IWechatApi _wechatApi;
        private readonly WechatServerDataContext _dbContext;
        public WechatController(IWechatApi wechatApi, WechatServerDataContext dbContext)
        {
            this._wechatApi = wechatApi;
            this._dbContext = dbContext;
            this._dbContext.WechatSessionInfos.Add(new WechatSessionInfo { Corpid = Guid.NewGuid().ToString(), Userid = Guid.NewGuid().ToString(), SessionKey = Guid.NewGuid().ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionAsync(string code)
        {
            var providerAccessToken = await this.GetProviderAccessToken();
            var code2SessionResponse = await this._wechatApi.GetWechatCode2SessionAsync(providerAccessToken, code);
            this.CheckWechatResponse(code2SessionResponse);

            return Ok(new { code2SessionResponse.Corpid, code2SessionResponse.Userid, code2SessionResponse.session_key });
        }

        private async Task<string> GetProviderAccessToken()
        {
            var providerAccessTokenResponse = await this._wechatApi.GetProviderAccessTokenAsync(new WechatProviderAccessTokenRequest { corpid = Consts.CorpId, provider_secret = Consts.ProviderSecret });
            this.CheckWechatResponse(providerAccessTokenResponse);
            return providerAccessTokenResponse.AccessToken;
        }

        private void CheckWechatResponse(WechatBasicResponse wechatBasicResponse)
        {
            if (!wechatBasicResponse.IsSuccess)
            {
                throw new Exception($"{wechatBasicResponse.ErrorCode}:{wechatBasicResponse.ErrorMessage}");
            }
        }
    }
}
