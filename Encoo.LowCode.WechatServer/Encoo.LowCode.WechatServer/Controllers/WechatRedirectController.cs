using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Models;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Controllers
{
    public class WechatRedirectController : Controller
    {

        private readonly IWechatApi _wechatApi;
        private readonly WechatServerDataContext _dbContext;
        public WechatRedirectController(IWechatApi wechatApi, WechatServerDataContext dbContext)
        {
            this._wechatApi = wechatApi;
            this._dbContext = dbContext;
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
            var suiteTicket = this._dbContext.WechatCaches.FirstOrDefault(e => e.Key == Consts.CacheKeySuiteTicket);
            if (suiteTicket == null)
            {
                throw new Exception("suiteTicket is null");
            }
            var suiteAccessTokenResponse = await this._wechatApi.GetSuiteAccessTokenAsync(new WechatSuiteAccessTokenRequest { suite_id = Consts.SuiteId, suite_secret = Consts.SuiteSecret, suite_ticket = suiteTicket.Value });
            this.CheckWechatResponse(suiteAccessTokenResponse);

            return suiteAccessTokenResponse.AccessToken;
        }

        public async Task<IActionResult> PreAuthCallback(string auth_code, int expires_in, string state)
        {
            var suiteTicket = this._dbContext.WechatCaches.FirstOrDefault(e => e.Key == Consts.CacheKeySuiteTicket);
            if (suiteTicket == null)
            {
                throw new Exception("suiteTicket is null");
            }
            var suiteAccessTokenResponse = await this._wechatApi.GetSuiteAccessTokenAsync(new WechatSuiteAccessTokenRequest { suite_id = Consts.SuiteId, suite_secret = Consts.SuiteSecret, suite_ticket = suiteTicket.Value });
            this.CheckWechatResponse(suiteAccessTokenResponse);

            var permanentCodeResponse = await this._wechatApi.GetPermanentCodeAsync(suiteAccessTokenResponse.AccessToken, new WechatPermanentCodeRequest { auth_code =auth_code});
            this.CheckWechatResponse(permanentCodeResponse);

            this._dbContext.WechatPermanentCodes.Add(new WechatPermanentCode { CorpId = permanentCodeResponse.AuthCorpInfo.Corpid, SuiteId = Consts.SuiteId, PermanentCode = permanentCodeResponse.PermanentCode });
            this._dbContext.SaveChanges();
            return View();
        }

        public async Task<IActionResult> WebLogin()
        {
            var redirectCallBackUrl = "https://consoletest.bottime.com/wechatserver/WechatRedirect/WebLoginCallback";
            redirectCallBackUrl = System.Web.HttpUtility.UrlEncode(redirectCallBackUrl, System.Text.Encoding.UTF8);
            var url = string.Format(Consts.WebOauth2Url, Consts.CorpId, redirectCallBackUrl);
            return Redirect(url);
        }

        public async Task<IActionResult> WebLoginCallback(string code, string state)
        {
            var suiteAccessToken = await this.GetSuiteAccessToken();
            var userinfoResponse = await this._wechatApi.GetAuthUserInfo(suiteAccessToken, code);
            this.CheckWechatResponse(userinfoResponse);
            return Ok();
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
                //try
                //{
                //    var userDetailInfo = await this._wechatApi.GetAuthUserDetailInfo(suiteAccessToken, new WechatUserDetailInfoRequest { user_ticket = userinfoResponse.user_ticket });
                //    this.CheckWechatResponse(userDetailInfo);
                //    ViewBag.UserDetailInfo = JsonConvert.SerializeObject(userDetailInfo);
                //}
                //catch (Exception e)
                //{
                //    ViewBag.UserDetailInfo = e.Message;
                //}
                ViewBag.Departments = await this.GetDepartments(userinfoResponse.CorpId);

                var permanent_code = this._dbContext.WechatPermanentCodes.OrderBy(e => e.Id).LastOrDefault();
                var suite_access_token = await this.GetSuiteAccessToken();
                var corp_access_token = await this._wechatApi.GetCorpAccessTokenAsync(suite_access_token, new WechatCropAccessTokenRequest { auth_corpid = userinfoResponse.CorpId, permanent_code = permanent_code.PermanentCode });
                this.CheckWechatResponse(corp_access_token);

                var userInfoResponse = await this._wechatApi.GetDepartmentUserInfo(corp_access_token.AccessToken, 1, 1);
                this.CheckWechatResponse(userInfoResponse);
                ViewBag.Users = userInfoResponse.userlist;

            }
            catch (Exception e)
            {
                ViewBag.UserInfo = e.Message;
            }

            return View();
        }

        private async Task<List<WechatDepartmentInfo>> GetDepartments(string corpid)
        {
            var permanent_code = this._dbContext.WechatPermanentCodes.OrderBy(e=>e.Id).LastOrDefault();
            var suite_access_token = await this.GetSuiteAccessToken();
            var corp_access_token = await this._wechatApi.GetCorpAccessTokenAsync(suite_access_token, new WechatCropAccessTokenRequest { auth_corpid = corpid, permanent_code = permanent_code.PermanentCode });
            this.CheckWechatResponse(corp_access_token);
            var departmentResponse = await this._wechatApi.GetAuthDepartmentInfo(corp_access_token.AccessToken);
            this.CheckWechatResponse(departmentResponse);


            return departmentResponse.department;
        }

        private void CheckWechatResponse(WechatBasicResponse wechatBasicResponse)
        {
            this._dbContext.WechatCaches.Add(new WechatCache { Key = $"CheckWechatResponse_{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}", Value = JsonConvert.SerializeObject(wechatBasicResponse) });
            this._dbContext.SaveChanges();
            if (!wechatBasicResponse.IsSuccess)
            {
                throw new Exception($"{wechatBasicResponse.ErrorCode}:{wechatBasicResponse.ErrorMessage}");
            }
        }
    }
}
