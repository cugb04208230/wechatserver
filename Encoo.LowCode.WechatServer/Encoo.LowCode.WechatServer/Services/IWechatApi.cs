using Encoo.LowCode.WechatServer.Models;
using Refit;
using System.Threading.Tasks;

namespace Encoo.LowCode.WechatServer.Services
{
    public interface IWechatApi
    {

        /// <summary>
        /// get access_token from wechat 
        /// </summary>
        /// <returns></returns>
        [Post("/cgi-bin/gettoken")]
        Task<WechatProviderAccessTokenResponse> GetProviderAccessTokenAsync([Body] WechatProviderAccessTokenRequest body);

        /// <summary>
        /// get suite_access_token from wechat 
        /// </summary>
        /// <returns></returns>
        [Post("/cgi-bin/service/get_suite_token")]
        Task<WechatSuiteAccessTokenResponse> GetSuiteAccessTokenAsync([Body] WechatSuiteAccessTokenRequest body);

        /// <summary>
        /// get corp accesstoken form wechat 
        /// </summary>
        /// <param name="suite_access_token"></param>
        /// <returns></returns>
        [Post("/cgi-bin/service/get_corp_token?suite_access_token={suite_access_token}")]
        Task<WechatCropAccessTokenResponse> GetCorpAccessTokenAsync(string suite_access_token, [Body] WechatCropAccessTokenRequest body);

        /// <summary>
        /// 授权码转换为Session
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Get("/cgi-bin/miniprogram/jscode2session?access_token={access_token}&js_code={code}&grant_type=authorization_code")]
        Task<WechatCode2SessionResponse> GetWechatCode2SessionAsync(string access_token, string code);


        /// <summary>
        /// 获取企业预授权码
        /// </summary>
        /// <param name="suite_access_token"></param>
        /// <returns></returns>
        [Get("/cgi-bin/service/get_pre_auth_code?suite_access_token={suite_access_token}")]
        Task<WechatPreAuthCodeResponse> GetPreAuthCodeAsync(string suite_access_token);

        /// <summary>
        /// 配置企业预授权码
        /// </summary>
        /// <param name="suite_access_token"></param>
        /// <returns></returns>
        [Post("/cgi-bin/service/get_pre_auth_code?suite_access_token={suite_access_token}")]
        Task<WechatBasicResponse> SetPreAuthCodeSettingAsync(string suite_access_token,[Body] WechatPreAuthSettingRequest body);

        /// <summary>
        /// 获取企业永久授权码
        /// </summary>
        /// <param name="suite_access_token"></param>
        /// <param name="auth_code">临时授权码</param>
        /// <returns></returns>
        [Post("/cgi-bin/service/get_permanent_code?suite_access_token={suite_access_token}")]
        Task<WechatPermanentCodeResponse> GetPermanentCodeAsync(string suite_access_token, [Body] WechatPermanentCodeRequest body);


        /// <summary>
        /// 获取企业授权信息
        /// </summary>
        /// <param name="suite_access_token"></param>
        /// <param name="auth_corpid"></param>
        /// <param name="permanent_code"></param>
        /// <returns></returns>
        [Post("/cgi-bin/service/get_auth_info?suite_access_token={suite_access_token}")]
        Task<WechatAuthCorpInfoResponse> GetCorpAuthInfo(string suite_access_token, [Body] WechatAuthCorpInfoRequest body);

        [Get("/cgi-bin/service/getuserinfo3rd?suite_access_token={suite_access_token}&code={code}")]
        Task<WechatAuthUserInfoResponse> GetAuthUserInfo(string suite_access_token, string code);

        [Get("/cgi-bin/service/getuserdetail3rd?suite_access_token={suite_access_token}")]
        Task<WechatUserDetailInfoResponse> GetAuthUserDetailInfo(string suite_access_token,[Body] WechatUserDetailInfoRequest body);

        [Get("/cgi-bin/department/list?access_token={access_token}")]
        Task<WechatDepartmentInfoResponse> GetAuthDepartmentInfo(string access_token);

        [Get("/cgi-bin/user/list?access_token={access_token}&department_id={department_id}&fetch_child={fetch_child}")]
        Task<WechatUserInfoResponse> GetDepartmentUserInfo(string access_token, int department_id, int fetch_child = 1);
    }
}
