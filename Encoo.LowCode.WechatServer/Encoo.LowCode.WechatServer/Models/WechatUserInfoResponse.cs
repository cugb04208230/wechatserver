using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatUserInfoResponse : WechatBasicResponse
    {
        public List<WechatUserInfo> userlist { set; get; }
    }

    public class WechatUserInfo
    {
        public string userid { set; get; }

        public string name { set; get; }
        public List<int> department { set; get; }
        public List<int> order { set; get; }
        public string position { set; get; }
        public string mobile { set; get; }
        public string gender { set; get; }
        public string email { set; get; }
        public List<int> is_leader_in_dept { set; get; }
        public string avatar { set; get; }
        public string thumb_avatar { set; get; }
        public string telephone { set; get; }
        public string alias { set; get; }
        public int status { set; get; }
        public string address { set; get; }

        public string qr_code { set;get;}

        public int main_department { set; get; }

        public string open_userid { set; get; }
    }
}
