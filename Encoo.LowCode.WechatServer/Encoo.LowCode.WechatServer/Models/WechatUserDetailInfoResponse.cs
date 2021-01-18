using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatUserDetailInfoRequest
    {
        public string user_ticket { set; get; }
    }

    public class WechatUserDetailInfoResponse : WechatBasicResponse
    {
        public string corpid { set; get; }
        public string userid { set; get; }
        public string name { set; get; }
        public string gender { set; get; }
        public string avatar { set; get; }
        public string qr_code { set; get; }
    }
}
