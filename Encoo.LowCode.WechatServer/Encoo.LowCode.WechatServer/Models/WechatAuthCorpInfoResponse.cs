using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models
{
    public class WechatAuthCorpInfoRequest
    {
        public string auth_corpid { set; get; }

        public string permanent_code { set; get; }
    }

    public class WechatAuthCorpInfoResponse : WechatBasicResponse
    {
        [JsonProperty("dealer_corp_info")]
        public WechatDealerCropInfo DealerCropInfo { set; get; }

        [JsonProperty("auth_corp_info")]
        public WechatAuthCorpInfo AuthCorpInfo { set; get; }

        [JsonProperty("auth_info")]
        public WechatAuthInfo AuthInfo { set; get; }

    }

    /// <summary>
    /// 代理服务商企业信息。应用被代理后才有该信息
    /// </summary>
    public class WechatDealerCropInfo
    {
        [JsonProperty("corpid")]
        public string Corpid { set; get; }

        [JsonProperty("corp_name")]
        public string CorpName { set; get; }
    }

    /// <summary>
    /// 授权企业信息
    /// </summary>
    public class WechatAuthCorpInfo
    {
        [JsonProperty("corpid")]
        public string Corpid { set; get; }

        [JsonProperty("corp_name")]
        public string CorpName { set; get; }

        [JsonProperty("corp_type")]
        public string CorpType { set; get; }

        [JsonProperty("corp_square_logo_url")]
        public string CorpSquareLogoUrl { set; get; }

        [JsonProperty("corp_user_max")]
        public int CorpUserMax { set; get; }

        [JsonProperty("corp_agent_max")]
        public int CorpAgentMax { set; get; }

        [JsonProperty("corp_full_name")]
        public string CorpFullName { set; get; }

        [JsonProperty("verified_end_time")]
        public long VerifiedEndTime { set; get; }

        [JsonProperty("subject_type")]
        public int SubjectType { set; get; }

        [JsonProperty("corp_wxqrcode")]
        public string CorpWxqrcode { set; get; }

        [JsonProperty("corp_scale")]
        public string CorpScale { set; get; }

        [JsonProperty("corp_industry")]
        public string CorpIndustry { set; get; }

        [JsonProperty("corp_sub_industry")]
        public string CorpSubIndustry { set; get; }

        [JsonProperty("location")]
        public string Location { set; get; }
    }

    public class WechatAuthInfo
    {
        public List<WechatAuthAgentInfo> agent { set; get; }
    }

    public class WechatAuthAgentInfo
    {
        public int agentid { set; get; }

        public string name { set; get; }

        public string round_logo_url { set; get; }
        public string square_logo_url { set; get; }
        public int appid { set; get; }

        public WechatAuthAgentPrivilegeInfo privilege { set; get; }

        public WechatAuthAgentSharedFromInfo shared_from { set; get; }
    }

    public class WechatAuthAgentPrivilegeInfo
    {
        public int level { set; get; }

        public List<int> allow_party { set; get; }

        public List<string> allow_user { set; get; }
        public List<int> allow_tag { set; get; }
        public List<int> extra_party { set; get; }
        public List<string> extra_user { set; get; }
        public List<int> extra_tag { set; get; }

    }

    public class WechatAuthAgentSharedFromInfo
    {
        public string corpid { set; get; }
    }
}
