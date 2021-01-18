using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatCode2SessionResponse : WechatBasicResponse
    {
        [JsonProperty("corpid")]
        public string Corpid { set; get; }

        [JsonProperty("userid")]
        public string Userid { set; get; }

        [JsonProperty("session_key")]
        public string session_key { set; get; }
    }
}
