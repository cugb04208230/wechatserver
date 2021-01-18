using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatPreAuthSettingRequest
    {
        public string pre_auth_code { set; get; }

        public WechatPreAuthSettingInfo session_info { set; get; } = new WechatPreAuthSettingInfo();
    }

    public class WechatPreAuthSettingInfo
    {
        public int auth_type { set; get; } = 1;
    }
}
