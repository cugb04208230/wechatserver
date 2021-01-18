using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Models;
using Encoo.LowCode.WechatServer.Models.Callback;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Encoo.LowCode.WechatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WechatCallbackController : ControllerBase
    {
        private readonly IWechatApi _wechatApi;
        private readonly WechatServerDataContext _dbContext;
        public WechatCallbackController(IWechatApi wechatApi, WechatServerDataContext dbContext)
        {
            this._wechatApi = wechatApi;
            this._dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            string msg_signature = this.Request.Query["msg_signature"];
            string nonce = this.Request.Query["nonce"];
            string timestamp = this.Request.Query["timestamp"];
            var readBody = await this.BodyModel();
            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(Consts.CallbackToken, Consts.CallbackEncodingAesKey, Consts.SuiteId);
            string sMsg = "";  // 解析之后的明文
            var ret = wxcpt.DecryptMsg(msg_signature, timestamp, nonce, readBody, ref sMsg);
            if (ret != 0)
            {
                Console.WriteLine("ERR: Decrypt Fail, ret: " + ret);
                return Ok("success");
            }
            this._dbContext.WechatCaches.Add(new WechatCache() { Value = sMsg, Key = Guid.NewGuid().ToString() });
            this._dbContext.SaveChanges();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sMsg);
            var root = doc.FirstChild;
            var infoType = root["InfoType"].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
            switch (infoType)
            {
                case "suite_ticket":
                    var suiteTicket = root["SuiteTicket"].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
                    var suiteTicketCache = this._dbContext.WechatCaches.FirstOrDefault(e => e.Key == Consts.CacheKeySuiteTicket);
                    if (suiteTicketCache == null)
                    {
                        this._dbContext.WechatCaches.Add(new WechatCache() { Value = suiteTicket, Key = Consts.CacheKeySuiteTicket });
                    }
                    else
                    {
                        suiteTicketCache.Value = suiteTicket;
                    }
                    this._dbContext.SaveChanges();
                    break;
                case "create_auth":
                    var authCodeCreate = root["AuthCode"].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
                    var suiteIdCreate = root["SuiteId"].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
                    var authCodeCache = this._dbContext.WechatCaches.FirstOrDefault(e => e.Key == Consts.CacheKeyAuthCode);
                    if (authCodeCache == null)
                    {
                        this._dbContext.WechatCaches.Add(new WechatCache() { Value = authCodeCreate, Key = Consts.CacheKeyAuthCode });
                    }
                    else
                    {
                        authCodeCache.Value = authCodeCreate;
                    }
                    this._dbContext.SaveChanges();

                    var permanentCodeResponse = await this._wechatApi.GetPermanentCodeAsync(await this.GetSuiteAccessToken(), authCodeCreate);
                    this._dbContext.WechatCaches.Add(new WechatCache() { Value = JsonConvert.SerializeObject(permanentCodeResponse), Key = Guid.NewGuid().ToString() });
                    this._dbContext.SaveChanges();
                    this.CheckWechatResponse(permanentCodeResponse);

                    this._dbContext.WechatPermanentCodes.Add(new WechatPermanentCode { CorpId = permanentCodeResponse.AuthCorpInfo.Corpid, SuiteId = Consts.SuiteId, PermanentCode = permanentCodeResponse.PermanentCode });
                    
                    this._dbContext.SaveChanges();

                    break;
                case "change_auth":
                    break;
                case "cancel_auth":
                    var authCodeCacheCancel = this._dbContext.WechatCaches.FirstOrDefault(e => e.Key == Consts.CacheKeyAuthCode);
                    if (authCodeCacheCancel != null)
                    {
                        this._dbContext.WechatCaches.Remove(authCodeCacheCancel);
                        this._dbContext.SaveChanges();
                    }
                    break;
                default:
                    break;

            }

            return Ok("success");
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string msg_signature, string nonce, string timestamp, string echostr)
        {
            this._dbContext.WechatCaches.Add(new WechatCache() { Value = $"Get has been call ,the query is {JsonConvert.SerializeObject(new string[] { msg_signature, nonce, timestamp, echostr })}", Key = Guid.NewGuid().ToString() });
            this._dbContext.SaveChanges();
            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(Consts.CallbackToken, Consts.CallbackEncodingAesKey, Consts.CorpId);
            string sEchoStr = "";
            var ret = wxcpt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref sEchoStr);
            if (ret != 0)
            {
                return Ok(false);
            }
            return Ok(sEchoStr);
        }

        private CallbackBasicModel ConvertToBizModel(CallbackBasicModel basicModel)
        {
            return basicModel.InfoType switch
            {
                "" => (SuiteTicketCallbackModel)basicModel,
                _ => basicModel
            };
        }


        private void CheckWechatResponse(WechatBasicResponse wechatBasicResponse)
        {
            if (!wechatBasicResponse.IsSuccess)
            {
                throw new Exception($"{wechatBasicResponse.ErrorCode}:{wechatBasicResponse.ErrorMessage}");
            }
        }

        public async Task<string> BodyModel()
        {
            string t = string.Empty;

            try
            {
                Request.EnableBuffering();
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string body = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;//以后可以重复读取
                    t = body;
                }
            }
            catch (Exception)
            {
                t = default;
            }
            return t;
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

    }

    public class CallbackPostBody
    {
        public string ToUserName { set; get; }

        public string AgentID { set; get; }

        public string Encrypt { set; get; }


        /// <summary>
        /// 回调信息类型
        /// </summary>
        public string InfoType { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { set; get; }

        /// <summary>
        /// 第三方应用的SuiteId
        /// </summary>
        public string SuiteId { set; get; }

        /// <summary>
        /// 第三方应用临时票据
        /// </summary>
        public string SuiteTicket { set; get; }
    }
}
