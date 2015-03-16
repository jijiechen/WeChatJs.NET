using System;
using WeChatJs.Providers;

namespace WeChatJs.Services
{
    public interface IWeChatServices
    {
        void Setup(IWeChat wechat, TimeSpan cacheDuration);
        string GetTicket(string accessToken);
        string GetAccessToken(string appId, string appSecret);
    }
}