using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatBasicResponse
    {
        [JsonProperty("errcode")]
        public int ErrorCode { set; get; }

        /// <summary>
        /// the boolean result of errorcode 
        /// </summary>
        public bool IsSuccess => ErrorCode == 0;

        [JsonProperty("errmsg")]
        public string ErrorMessage { set; get; }
    }
}
