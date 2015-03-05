using System.Web;
using System.Web.Mvc;
using WcShareService.Models;
using WcShareService.Services;
using WcShareService.Utils;

namespace WcShareService.Controllers
{
    public class WeChatSignatureController : Controller
    {
        //
        // GET: /WeChatSignature/

        public ActionResult Index(string appId, string appSecret, string url)
        {
            Parameters.RequireNotEmpty("appId", appId);
            Parameters.RequireNotEmpty("appSecret", appSecret);

            var wechat = HttpContext.Items[MvcApplication.ContextKey_WeChatService] as IWeChatServices;
            var signatureGenerator = HttpContext.Items[MvcApplication.ContextKey_SignatureGenerator] as ISignatureGenerator;

            var accessToken = wechat.GetAccessToken(appId, appSecret);
            var ticket = wechat.GetTicket(accessToken);
            var jsConfig = signatureGenerator.GenerateWeChatJsConfigurationWithSignature(ticket, url);
            jsConfig.AppId = appId;

            return Content(SignatureScriptContent(jsConfig) ,"text/javascript");
        }


        private static string SignatureScriptContent(WeChatJsConfiguration jsConfig)
        {
            const string script = @"if(window.jWeixin){{
    jWeixin.config({{
	    appId: '{0}',
	    timestamp: {1},
	    nonceStr: '{2}',
	    signature: '{3}',
        debug: {4},
	    jsApiList: [
		    'checkJsApi',
		    'onMenuShareTimeline',
		    'onMenuShareAppMessage',
		    'onMenuShareQQ',
		    'onMenuShareWeibo'
		    ]
    }}); 

    if(window.weixinShareData){{
	    jWeixin.ready(function () {{
		    jWeixin.onMenuShareAppMessage(weixinShareData);
		    jWeixin.onMenuShareTimeline(weixinShareData);
		    jWeixin.onMenuShareQQ(weixinShareData);
	    }});
    }}
}}";
            return string.Format(script,
                jsConfig.AppId,
                jsConfig.Timestamp,
                jsConfig.NonceString,
                jsConfig.Signature,
                jsConfig.DebugMode.ToString().ToLower());
        }

    }
}
