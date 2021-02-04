using Encoo.LowCode.WechatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Encoo.LowCode.WechatServer.Services
{
    public class CacheService
    {
        public string SuiteTicket { set; get; }

        public Dictionary<string, string> PermanentCodes { set; get; }

        public Dictionary<string, List<WechatDepartmentInfo>> Departments { set; get; }
        public Dictionary<string, List<WechatUserInfo>> DepartmentUsers { set; get; }

        public Dictionary<string, WechatAuthUserInfoResponse> Tickets { set; get; }

        private readonly IWechatApi _wechatApi;

        public CacheService(IWechatApi wechatApi)
        {
            this._wechatApi = wechatApi;
            PermanentCodes = new Dictionary<string, string>();
            Departments = new Dictionary<string, List<WechatDepartmentInfo>>();
            DepartmentUsers = new Dictionary<string, List<WechatUserInfo>>();
            Tickets = new Dictionary<string, WechatAuthUserInfoResponse>();
        }

        public void CallbackAsync(string msgSignature, string timestamp, string nonce, string body)
        {
            var callbackToken = Consts.CallbackToken;
            var callbackEncodingAesKey = Consts.CallbackEncodingAesKey;
            var suiteId = Consts.SuiteId;
            Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(callbackToken, callbackEncodingAesKey, suiteId);
            string sMsg = "";
            var ret = wxcpt.DecryptMsg(msgSignature, timestamp, nonce, body, ref sMsg);
            if (ret != 0)
            {
                return;
            }
            var infoType = this.AnalysisProperty(sMsg, "InfoType");
            switch (infoType)
            {
                case "suite_ticket":
                    this.InitSuiteTicketAsync(sMsg);
                    break;
                case "create_auth":
                    this.CreatePermanentCodeAsync(sMsg);
                    break;
                case "change_auth":
                    // need to compare by ourself
                    break;
                case "cancel_auth":
                    this.CancelPermanentCodeAsync(sMsg);
                    break;
                case "change_contact":
                    this.ChangeContactAsync(sMsg);
                    break;
                case "share_agent_change":
                    // todo if the company is a multi-company
                    break;
                default:
                    return;
            }
        }

        public void InitSuiteTicketAsync(string content)
        {
            var suiteId = this.AnalysisProperty(content, "SuiteId");
            var suiteTicket = this.AnalysisProperty(content, "SuiteTicket");

            SuiteTicket = suiteTicket;
        }

        public void CreatePermanentCodeAsync(string content)
        {
            var suiteId = this.AnalysisProperty(content, "SuiteId");
            var authCode = this.AnalysisProperty(content, "AuthCode");

            var suiteAccessToken = this._wechatApi.GetSuiteAccessTokenAsync(new Models.WechatSuiteAccessTokenRequest { suite_ticket = SuiteTicket }).Result;

            var response = this._wechatApi.GetPermanentCodeAsync(suiteAccessToken.AccessToken, new Models.WechatPermanentCodeRequest { auth_code = authCode }).Result;

            if (PermanentCodes.ContainsKey(response.AuthCorpInfo.Corpid))
            {
                PermanentCodes[response.AuthCorpInfo.Corpid] = response.PermanentCode;
            }
            else
            {
                PermanentCodes.Add(response.AuthCorpInfo.Corpid, response.PermanentCode);
            }

        }

        public void CancelPermanentCodeAsync(string content)
        {
            var suiteId = this.AnalysisProperty(content, "SuiteId");
            var authCorpId = this.AnalysisProperty(content, "AuthCorpId");
            if (PermanentCodes.ContainsKey(authCorpId))
            {
                PermanentCodes.Remove(authCorpId);
            }
            if (Departments.ContainsKey(authCorpId))
            {
                Departments.Remove(authCorpId);
            }
            if (DepartmentUsers.ContainsKey(authCorpId))
            {
                DepartmentUsers.Remove(authCorpId);
            }
        }

        public void ChangeContactAsync(string content)
        {
            var changeType = this.AnalysisProperty(content, "ChangeType");
            switch (changeType)
            {
                case "create_user":
                case "update_user":
                case "delete_user":
                    // todo update all organization
                    break;
                case "create_party":
                case "update_party":
                case "delete_party":
                    // todo update all organization
                    break;
                case "update_tag":
                    break;
            }
            SyncContact();
        }

        public void SyncContact()
        {
            var suiteAccessToken = this._wechatApi.GetSuiteAccessTokenAsync(new Models.WechatSuiteAccessTokenRequest { suite_ticket = SuiteTicket }).Result;
            foreach (var kv in PermanentCodes) 
            {
                var corpid = kv.Key;
                var permanentCode = kv.Value;
                var corp_access_token = _wechatApi.GetCorpAccessTokenAsync(suiteAccessToken.AccessToken, new WechatCropAccessTokenRequest { auth_corpid = corpid, permanent_code = permanentCode }).Result;

                var response = this._wechatApi.GetAuthDepartmentInfo(corp_access_token.AccessToken).Result;

                if (Departments.ContainsKey(corpid))
                {
                    Departments[corpid] = response.department;
                }
                else
                {
                    Departments.Add(corpid, response.department);
                }

                var userreponse = this._wechatApi.GetDepartmentUserInfo(corp_access_token.AccessToken, 1, 1).Result;
                if (DepartmentUsers.ContainsKey(corpid))
                {
                    DepartmentUsers[corpid] = userreponse.userlist;
                }
                else
                {
                    DepartmentUsers.Add(corpid, userreponse.userlist);
                }

            }
        }

        private string AnalysisProperty(string content, string key)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            var root = doc.FirstChild;
            return root[key].InnerText.Replace("<![CDATA[", "").Replace("]]", "");
        }
    }
}
