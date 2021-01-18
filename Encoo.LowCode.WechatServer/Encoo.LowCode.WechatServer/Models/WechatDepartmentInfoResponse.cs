using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatDepartmentInfoResponse : WechatBasicResponse
    {
        public List<WechatDepartmentInfo> department { set; get; }
    }

    public class WechatDepartmentInfo
    {
        public int id { set; get; }

        public string name { set; get; }

        public string name_en { set; get; }

        public int parent_id { set; get; }

        public int order { set; get; }
    }
}
