using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Models.Callback
{
    public class CallbackBasicModel
    {
        /// <summary>
        /// 回调信息类型
        /// </summary>
        public string InfoType { set; get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimeStamp { set; get; }

        /// <summary>
        /// 第三方应用的SuiteId
        /// </summary>
        public string SuiteId { set; get; }
    }

    public class SuiteTicketCallbackModel : CallbackBasicModel
    {
        public SuiteTicketCallbackModel()
        {
            InfoType = "suite_ticket";
        }

        /// <summary>
        /// 应用临时票据
        /// </summary>
        public string SuiteTicket { set; get; }
    }

    /// <summary>
    /// 授权成功通知
    /// 从企业微信应用市场发起授权时，企业微信后台会推送授权成功通知。
    /// </summary>
    public class AuthCreateCallbackModel : CallbackBasicModel
    {
        public AuthCreateCallbackModel()
        {
            InfoType = "create_auth";
        }

        /// <summary>
        /// 临时授权码
        /// </summary>
        public string AuthCode { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AuthChangeCallbackModel : CallbackBasicModel
    {
        public AuthChangeCallbackModel()
        {
            InfoType = "change_auth";
        }

        /// <summary>
        /// 授权企业Id
        /// </summary>
        public string AuthCorpId { set; get; }
    }

    /// <summary>
    /// 取消授权回调
    /// </summary>
    public class AuthCancelCallbackModel : CallbackBasicModel
    {
        public AuthCancelCallbackModel()
        {
            InfoType = "cancel_auth";
        }

        /// <summary>
        /// 授权企业Id
        /// </summary>
        public string AuthCorpId { set; get; }
    }
}
