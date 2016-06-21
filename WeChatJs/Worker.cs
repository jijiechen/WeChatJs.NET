using System;
#if net40
    using System.Web;
#else
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
#endif
using WeChatJs.Services;
using WeChatJs.Utils;


namespace WeChatJs
{
    public static class Worker
    {
        public const string ContextKey_WeChatService = "instance.WeChatService";
        public const string ContextKey_SignatureGenerator = "instance.SignatureGenerator";
        public static TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(7000);

        public static string GenerateBridgeScriptWithSignature(string appId, string appSecret, string url = null, bool debug = false)
        {
            return GenerateBridgeScriptWithSignature(null, appId, appSecret, url, debug);
        }

        public static string GenerateBridgeScriptWithSignature(IServiceProvider services, string appId, string appSecret, string url = null, bool debug = false)
        {
            var config = Sign(services, appId, appSecret, url);
            config.DebugMode = debug;

            return BuildSignatureScriptContent(config);
        }

        public static WeChatJsConfiguration Sign(string appId, string appSecret, string url = null)
        {
            return Sign(null, appId, appSecret, url);
        }

        public static WeChatJsConfiguration Sign(IServiceProvider services, string appId, string appSecret, string url = null)
        {
            Parameters.RequireNotEmpty("appId", appId);
            Parameters.RequireNotEmpty("appSecret", appSecret);

            var wechat = GetWeChatServices(services);
            var accessToken = wechat.GetAccessToken(appId, appSecret);
            var ticket = wechat.GetTicket(accessToken);

            var actualUrl = StripUrl(services, url);
            var signatureGenerator = GetSignatureGenerator(services);
            var jsConfig = signatureGenerator.GenerateWeChatJsConfigurationWithSignature(ticket, actualUrl);
            if (string.IsNullOrWhiteSpace(jsConfig.Url))
            {
                jsConfig.Url = actualUrl;
            }
            jsConfig.AppId = appId;

            return jsConfig;
        }

        private static string StripUrl(IServiceProvider services, string url) {
            Uri actualUri;


            if (string.IsNullOrWhiteSpace(url))
            {
#if net40
                if (HttpContext.Current == null)
                {
                    Parameters.RequireNotEmpty("url", HttpContext.Current);
                }

                return HttpContext.Current.Request.Url.ToString();
#else
                var httpContextAccessor = TryGetService<IHttpContextAccessor>(services);
                if(httpContextAccessor != null)
                {
                    return UriHelper.GetDisplayUrl(httpContextAccessor.HttpContext.Request);
                }
#endif
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out actualUri))
            {
                var uriBuilder = new UriBuilder(actualUri);
                uriBuilder.Fragment = null;

                // remove 80 port if possible
                // see http://skumarone.blogspot.com/2009/04/difference-between-uribuildertostring.html
                return uriBuilder.Uri.ToString();
            }

            throw new ArgumentException("Invalid url", "url");
        }

        public static string BuildSignatureScriptContent(WeChatJsConfiguration jsConfig)
        {
            const string script = @"; function configWeixinJs ( cfg, dontSetupWeixin ){{
    cfg = cfg || {{}};
    cfg.appId = '{0}'; cfg.timestamp = {1}; cfg.nonceStr = '{2}'; cfg.signature = '{3}'; cfg.debug = {4};

	if(!cfg.jsApiList || !cfg.jsApiList.length) {{
        cfg.jsApiList = [ 'checkJsApi', 'onMenuShareTimeline', 'onMenuShareAppMessage', 'onMenuShareQQ', 'onMenuShareWeibo' ];
    }}
    
    if( window.jWeixin && !dontSetupWeixin) {{
        jWeixin.config( cfg ); 
        if( window.weixinShareData ){{
	        jWeixin.ready ( function () {{
		        jWeixin.onMenuShareAppMessage(weixinShareData);
		        jWeixin.onMenuShareTimeline(weixinShareData);
		        jWeixin.onMenuShareQQ(weixinShareData);
	        }});
        }}
    }}else if( !window.jWeixin && window.console ){{
        console.log( 'WeChatJs: please put this WeChatJs script after WeChat\'s official sdk script.' );
    }}

    return cfg;
}} configWeixinJs( null, {5} );";
            return string.Format(script,
                jsConfig.AppId,
                jsConfig.Timestamp,
                jsConfig.NonceString,
                jsConfig.Signature,
                jsConfig.DebugMode.ToString().ToLower(),
                jsConfig.DontSetupWeChatOnGeneratingScript.ToString().ToLower());
        }
        
        private static IWeChatServices GetWeChatServices(IServiceProvider services){
            IWeChatServices wechat = null;

#if net40
            var httpContext = HttpContext.Current;
            if (httpContext != null) { 
                wechat = httpContext.Items[ContextKey_WeChatService] as IWeChatServices;
            }
#endif
            if (services != null)
            {
                wechat = TryGetService<IWeChatServices>(services);
            }

            if (wechat == null)
            {
                var wechatProvider = new Providers.TencentWeChat();
                wechat = new Services.Impl.WeChatService();
                wechat.Setup(wechatProvider, DefaultCacheDuration);
            }
            
            return wechat;
        }

        private static ISignatureGenerator GetSignatureGenerator(IServiceProvider services)
        {
            ISignatureGenerator sigGenerator = null;
#if net40
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                sigGenerator = httpContext.Items[ContextKey_SignatureGenerator] as ISignatureGenerator;
            }
#endif
            if (services != null)
            {
                sigGenerator = TryGetService<ISignatureGenerator>(services);
            }

            if (sigGenerator == null)
            {
                sigGenerator = new Services.Impl.SignatureGenerator();
            }
            return sigGenerator;
        }

        private static T TryGetService<T>(IServiceProvider services) where T: class
        {
            // todo: check if services.GetService could throw some kind of exceptions
            return services == null ? null : services.GetService(typeof (T)) as T;
        }
    }
}