using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatPreAuthCodeResponse : WechatBasicResponse
    {
        [JsonProperty("pre_auth_code")]
        public string PreAuthCode { set; get; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { set; get; }
    }
}
