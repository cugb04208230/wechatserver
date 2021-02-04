using Encoo.LowCode.WechatServer.Models;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Controllers
{
    public class WechatRedirectController : Controller
    {

        private readonly IWechatApi _wechatApi;
        private readonly CacheService _cacheService;
        public WechatRedirectController(CacheService cacheService, IWechatApi wechatApi)
        {
            this._wechatApi = wechatApi;
            this._cacheService = cacheService;
        }

        public async Task<IActionResult> PreAuth()
        {
            var suiteAccessToken = await this.GetSuiteAccessToken();
            var preAuthCodeResponse = await this._wechatApi.GetPreAuthCodeAsync(suiteAccessToken);
            this.CheckWechatResponse(preAuthCodeResponse);
            //todo 正式环境需要作为可配置auth_type
            var preAuthCodeSettingResponse = await this._wechatApi.SetPreAuthCodeSettingAsync(suiteAccessToken, new WechatPreAuthSettingRequest { pre_auth_code = preAuthCodeResponse.PreAuthCode });
            this.CheckWechatResponse(preAuthCodeSettingResponse);

            var redirectCallBackUrl = "https://consoletest.bottime.com/wechatserver/WechatRedirect/PreAuthCallback";
            redirectCallBackUrl = System.Web.HttpUtility.UrlEncode(redirectCallBackUrl, System.Text.Encoding.UTF8);
            return Redirect($"https://open.work.weixin.qq.com/3rdapp/install?suite_id={Consts.SuiteId}&pre_auth_code={preAuthCodeResponse.PreAuthCode}&redirect_uri={redirectCallBackUrl}&state=acc");
        }

        private async Task<string> GetSuiteAccessToken()
        {
            var suiteAccessTokenResponse = await this._wechatApi.GetSuiteAccessTokenAsync(new WechatSuiteAccessTokenRequest { suite_id = Consts.SuiteId, suite_secret = Consts.SuiteSecret, suite_ticket = this._cacheService.SuiteTicket });
            this.CheckWechatResponse(suiteAccessTokenResponse);

            return suiteAccessTokenResponse.AccessToken;
        }

        public async Task<IActionResult> PreAuthCallback(string auth_code, int expires_in, string state)
        {
            var suiteAccessTokenResponse = await this._wechatApi.GetSuiteAccessTokenAsync(new WechatSuiteAccessTokenRequest { suite_id = Consts.SuiteId, suite_secret = Consts.SuiteSecret, suite_ticket = _cacheService.SuiteTicket });
            this.CheckWechatResponse(suiteAccessTokenResponse);

            var permanentCodeResponse = await this._wechatApi.GetPermanentCodeAsync(suiteAccessTokenResponse.AccessToken, new WechatPermanentCodeRequest { auth_code = auth_code });
            this.CheckWechatResponse(permanentCodeResponse);

            if (_cacheService.PermanentCodes.ContainsKey(permanentCodeResponse.AuthCorpInfo.Corpid))
            {
                _cacheService.PermanentCodes[permanentCodeResponse.AuthCorpInfo.Corpid] = permanentCodeResponse.PermanentCode;
            }
            else
                _cacheService.PermanentCodes.Add(permanentCodeResponse.AuthCorpInfo.Corpid, permanentCodeResponse.PermanentCode);

            return View();
        }

        public async Task<IActionResult> WebLogin()
        {
            var redirectCallBackUrl = "https://consoletest.bottime.com/wechatserver/WechatRedirect/WebLoginCallback";
            redirectCallBackUrl = System.Web.HttpUtility.UrlEncode(redirectCallBackUrl, System.Text.Encoding.UTF8);
            var url = string.Format(Consts.WebOauth2Url, Consts.CorpId, redirectCallBackUrl, Consts.SuiteId);
            return Redirect(url);
        }

        public async Task<IActionResult> WebLoginCallback(string code, string state)
        {
            var suiteAccessToken = await this.GetSuiteAccessToken();
            var userinfoResponse = await this._wechatApi.GetAuthUserInfo(suiteAccessToken, code);
            this.CheckWechatResponse(userinfoResponse);
            return Ok($"{code},{suiteAccessToken},{JsonConvert.SerializeObject(userinfoResponse)}");
        }

        public async Task<IActionResult> AppLogin()
        {
            var redirectCallBackUrl = "https://consoletest.bottime.com/wechatserver/WechatRedirect/AppLoginCallback";
            redirectCallBackUrl = System.Web.HttpUtility.UrlEncode(redirectCallBackUrl, System.Text.Encoding.UTF8);
            var url = string.Format(Consts.AppOauth2Url, Consts.SuiteId, redirectCallBackUrl);
            return Redirect(url);
        }

        public async Task<IActionResult> AppLoginCallback(string code, string state)
        {
            var suiteAccessToken = await this.GetSuiteAccessToken();
            try
            {
                var userinfoResponse = await this._wechatApi.GetAuthUserInfo(suiteAccessToken, code);
                this.CheckWechatResponse(userinfoResponse);
                ViewBag.UserInfo = userinfoResponse;
                var key = Guid.NewGuid().ToString();
                _cacheService.Tickets.Add(key, userinfoResponse);
                ViewBag.Token = key;
            }
            catch (Exception e)
            {
                ViewBag.UserInfo = e.Message;
            }

            return View();
        }

        public async Task<IActionResult> QrLogin(string callbackurl,string state)
        {
            callbackurl = System.Web.HttpUtility.UrlEncode(callbackurl, System.Text.Encoding.UTF8);
            var url = $"https://consoletest.bottime.com/wechatserver/WechatRedirect/QrLoginCallback?callbackurl={callbackurl}";
            url = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
            ViewBag.Url = string.Format(Consts.QrLoginUrl, Consts.CorpId, url,state, "member");
            return View();
        }

        public async Task<IActionResult> QrLoginCallback(string callbackurl,string auth_code, string state)
        {
            var suiteAccessToken = await _wechatApi.GetProviderAccessTokenAsync(new WechatProviderAccessTokenRequest { });
                this.CheckWechatResponse(suiteAccessToken);
                var userinfoResponse = await this._wechatApi.GetLoginAuthUserInfo(suiteAccessToken.AccessToken, new LoginUserRequest { auth_code=auth_code});
                this.CheckWechatResponse(userinfoResponse);
                var key = Guid.NewGuid().ToString();
                _cacheService.Tickets.Add(key, new WechatAuthUserInfoResponse
                {
                    UserId = userinfoResponse.user_info.userid,
                    open_userid = userinfoResponse.user_info.open_userid,
                    CorpId = userinfoResponse.corp_info.corpid,
                    user_ticket = userinfoResponse.user_info.name
                }) ;
            if (!callbackurl.Contains("?"))
            {
                callbackurl = callbackurl + "?";
            }
            callbackurl += $"&state={state}";
            callbackurl += $"&ticket={key}";
            ViewBag.Url = callbackurl;
            return View();
        }

        private void CheckWechatResponse(WechatBasicResponse wechatBasicResponse)
        {
            Console.WriteLine(JsonConvert.SerializeObject(wechatBasicResponse));
            if (!wechatBasicResponse.IsSuccess)
            {
                throw new Exception($"{wechatBasicResponse.ErrorCode}:{wechatBasicResponse.ErrorMessage}");
            }
        }
    }
}
