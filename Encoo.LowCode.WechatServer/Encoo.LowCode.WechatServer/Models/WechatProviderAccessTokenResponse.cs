using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{

    public class WechatProviderAccessTokenRequest
    {
        public string corpid { set; get; } = Consts.CorpId;
        public string provider_secret { set; get; } = Consts.ProviderSecret;
    }

    public class WechatProviderAccessTokenResponse : WechatBasicResponse
    {
        [JsonProperty("provider_access_token")]
        public string AccessToken { set; get; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { set; get; }
    }
}
