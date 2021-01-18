using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatAuthUserInfoResponse : WechatBasicResponse
    {
        public string CorpId { set; get; }

        public string UserId { set; get; }
        public string DeviceId { set; get; }
        public string user_ticket { set; get; }
        public int expires_in { set; get; }
        public string open_userid { set; get; }
    }
}
