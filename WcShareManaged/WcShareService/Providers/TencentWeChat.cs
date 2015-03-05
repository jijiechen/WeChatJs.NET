using Newtonsoft.Json;
using System;
using System.Net;

namespace WcShareService.Providers
{
    public class TencentWeChat:IWeChat
    {
        const string AccessTokenUrlFormat = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        const string JsTicketUrlFormat = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";


        public string ProvideAccessToken(string appId, string appSecret)
        {
            var accessTokenUrl = string.Format(AccessTokenUrlFormat, Uri.EscapeDataString(appId), Uri.EscapeDataString(appSecret));
            var wechatResponse = new WebClient().DownloadString(accessTokenUrl);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(wechatResponse);
            return jsonObject.access_token.ToString();
        }

        public string ProvideJsTicket(string accessToken)
        {
            var jsTicketUrl = string.Format(JsTicketUrlFormat, Uri.EscapeDataString(accessToken));
            var wechatResponse = new WebClient().DownloadString(jsTicketUrl);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(wechatResponse);
            return jsonObject.ticket.ToString();
        }
    }
}