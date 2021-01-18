using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{

    public class WechatPermanentCodeResponse : WechatAuthCorpInfoResponse
    {
        [JsonProperty("permanent_code")]
        public string PermanentCode { set; get; }

        [JsonProperty("access_token")]
        public string AccessToken { set; get; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { set; get; }

        [JsonProperty("expires_in")]
        public WechatAuthUserInfo AuthUserInfo { set; get; }

        [JsonProperty("register_code_info")]
        public WechatRegisterCodeInfo RegisterCodeInfo { set; get; }
    }

    public class WechatAuthUserInfo
    {
        [JsonProperty("userid")]
        public string UserId { set; get; }

        [JsonProperty("name")]
        public string Name { set; get; }

        [JsonProperty("avatar")]
        public string Avatar { set; get; }
    }

    public class WechatRegisterCodeInfo
    {
        [JsonProperty("RegisterCode")]
        public string register_code { set; get; }

        [JsonProperty("TemplateId")]
        public string template_id { set; get; }

        [JsonProperty("State")]
        public string state { set; get; }
    }
}
