using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WechatCallbackController : ControllerBase
    {
        private readonly IWechatApi _wechatApi;
        private readonly WechatServerDataContext _dbContext;
        private readonly CacheService _cacheService;
        public WechatCallbackController(IWechatApi wechatApi, WechatServerDataContext dbContext, CacheService cacheService)
        {
            this._wechatApi = wechatApi;
            this._dbContext = dbContext;
            this._cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            string msg_signature = this.Request.Query["msg_signature"];
            string nonce = this.Request.Query["nonce"];
            string timestamp = this.Request.Query["timestamp"];
            var readBody = await this.BodyModel();
            this._cacheService.CallbackAsync(msg_signature, timestamp, nonce, readBody);
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
