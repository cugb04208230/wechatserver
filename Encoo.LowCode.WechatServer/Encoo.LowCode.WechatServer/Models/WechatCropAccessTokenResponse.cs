using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{

    public class WechatCropAccessTokenRequest
    {
        public string auth_corpid { set; get; }
        public string permanent_code { set; get; }
    }
    public class WechatCropAccessTokenResponse : WechatBasicResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { set; get; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { set; get; }
    }
}
