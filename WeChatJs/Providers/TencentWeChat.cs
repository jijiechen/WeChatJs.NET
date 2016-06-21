using Newtonsoft.Json;
using System;
#if net40
using System.Net;
#else
    using System.Net.Http;
#endif


namespace WeChatJs.Providers
{
    public class TencentWeChat:IWeChat
    {
        const string AccessTokenUrlFormat = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        const string JsTicketUrlFormat = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";


        public string ProvideAccessToken(string appId, string appSecret)
        {
            var accessTokenUrl = string.Format(AccessTokenUrlFormat, Uri.EscapeDataString(appId), Uri.EscapeDataString(appSecret));
            string wechatResponse = DownloadFromUri(accessTokenUrl);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(wechatResponse);
            return jsonObject.access_token.ToString();
        }

        public string ProvideJsTicket(string accessToken)
        {
            var jsTicketUrl = string.Format(JsTicketUrlFormat, Uri.EscapeDataString(accessToken));
            var wechatResponse = DownloadFromUri(jsTicketUrl);

            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(wechatResponse);
            return jsonObject.ticket.ToString();
        }


        private static string DownloadFromUri(string accessTokenUrl)
        {
#if net40
            return new WebClient().DownloadString(accessTokenUrl);
#else
            using (var client = new HttpClient())
            using (var response = client.GetAsync(accessTokenUrl).Result)
            using (var content = response.Content)
            {
                return content.ReadAsStringAsync().Result;
            }
#endif
        }
    }
}