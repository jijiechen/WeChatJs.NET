using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WeChatJs.Providers;
using WeChatJs.Services;
using WeChatJs.Services.Impl;

namespace WeChatJs.SaasDemo
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWeChatServices WeChatServiceInstance;

        protected void Application_Start()
        {
            InitWeChatServices();
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            Context.Items[ WeChatJs.Worker.ContextKey_WeChatService ] = WeChatServiceInstance;
            Context.Items[ WeChatJs.Worker.ContextKey_SignatureGenerator ] = new SignatureGenerator();
        }

        private void InitWeChatServices()
        {
            var wechat = new TencentWeChat();
            WeChatServiceInstance = new WeChatService();
            WeChatServiceInstance.Setup(wechat, TimeSpan.FromMinutes(100) /* 微信要求缓存 120 分钟...这里仅要求缓存 100 分钟，以保证其正确性 */);
        }
    }
}