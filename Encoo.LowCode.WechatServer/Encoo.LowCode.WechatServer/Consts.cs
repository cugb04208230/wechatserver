using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer
{
    public class Consts
    {
        public const string CallbackToken = "YVmgtGxMrMXYgrgX3gkKqt";

        public const string CallbackEncodingAesKey = "L5OyjQ3AybbuKfwkx4IBMX1WZz5Bm2rUkvg8kHokHE4";

        public const string CorpId = "wwe4fe7dec42c79ac6";

        public const string ProviderSecret = "h9Y8mM9pH5_0MhDe-oblp1J9ESM-QCGnM_GuAucNIEa2vqkpMnvRUIaEpsbjYz-o";

        public const string SuiteId = "wwa9fab5c9fd8b9fec";

        public const string SuiteSecret = "e8vJmDFh3xU0sEcMq_sknurGE9WF7Hrc8aDGb8QieR0";

        public const string CacheKeySuiteTicket = "CacheKeySuiteTicket";
        public const string CacheKeyAuthCode = "CacheKeyAuthCode";

        /// <summary>
        /// 第三方应用内打开网页时的Uri string.format(Consts.AppOauth2Url,suiteid,UrlEncode(redirect_url))
        /// </summary>
        public const string AppOauth2Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect";
        /// <summary>
        /// 网页应用构建uri string.format(Consts.AppOauth2Url,suiteid,UrlEncode(redirect_url))
        /// </summary>
        public const string WebOauth2Url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&agentid={2}&state=STATE#wechat_redirect";

        public const string QrLoginUrl = "https://open.work.weixin.qq.com/wwopen/sso/3rd_qrConnect?appid={0}&redirect_uri={1}&state=encoo_web_login@qr.qywechat&usertype={2}";
    }
}
