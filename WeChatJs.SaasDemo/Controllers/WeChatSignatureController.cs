using System;
using System.Linq;
using System.Web.Mvc;
using WeChatJs.SaasDemo.DataStore;

namespace WeChatJs.SaasDemo.Controllers
{
    public class WeChatSignatureController : Controller
    {
        const string JavaScriptMimeType = "text/javascript";

        //
        // GET: /WeChatSignature/

        public ActionResult Index(string appId, string url, string debug, bool donotSetup)
        {
            if(string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(url)){
                return HttpNotFound();
            }

            var all = FromConfiguration.Credentials;
            var credential = all.FirstOrDefault(c => c.AppId == appId);
            if (credential == null)
            {
                return HttpNotFound();
            }

            var debugMode = !string.IsNullOrWhiteSpace(debug) && !debug.Equals("false", StringComparison.OrdinalIgnoreCase);
            return CreateResponseResult(credential, url, debugMode, donotSetup);
        }

        private ActionResult CreateResponseResult(WeChatCredential credential, string url, bool debug, bool donotSetup)
        {
            try
            {
                var config = Worker.Sign(credential.AppId, credential.AppSecret, url);
                config.DebugMode = debug;
                config.DontSetupWeChatOnGeneratingScript = donotSetup;

                var script = Worker.BuildSignatureScriptContent(config);
                return Content(script, JavaScriptMimeType);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("url"))
                {
                    return Content("window && window.console && window.console.log(\"invalid url\");", JavaScriptMimeType);
                }
            }
#if DEBUG
            catch(Exception ex)
            {
                return Content(ex.Message + "\r\n" + ex.StackTrace, "text/plain");
            }
#endif

            return new HttpStatusCodeResult(400);
        }


#if DEBUG
        public ActionResult Test(string appId, string appSecret, string url)
        {
            return Content(Worker.GenerateBridgeScriptWithSignature(appId, appSecret, url, true), JavaScriptMimeType);
        }
#endif

    }
}
