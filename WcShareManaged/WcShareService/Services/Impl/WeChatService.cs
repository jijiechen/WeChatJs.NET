using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WcShareService.Providers;

namespace WcShareService.Services.Impl
{
    public class WeChatService: IWeChatServices
    {
        private IWeChat _wechat;

        public void Setup(IWeChat wechat)
        {
            if (wechat == null)
            {
                throw new ArgumentNullException("wechat");
            }

            _wechat = wechat;
        }

        public string GetTicket(string appId)
        {
            throw new NotImplementedException();
        }

        public string GetAccessToken(string appId, string appSecret)
        {
            throw new NotImplementedException();
        }
    }
}