using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatSuiteAccessTokenRequest
    {
        public string suite_id { set; get; }
        public string suite_secret { set; get; }
        public string suite_ticket { set; get; }
    }

    public class WechatSuiteAccessTokenResponse : WechatBasicResponse
    {
        [JsonProperty("suite_access_token")]
        public string AccessToken { set; get; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { set; get; }
    }
}
