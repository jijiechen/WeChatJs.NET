using System.Web;
using System.Web.Mvc;
using WeChatJs;
using System.Linq;
using System;

namespace WeChatJs.SaasDemo.Controllers
{
    public class WeChatSignatureController : Controller
    {
        const string JavaScriptMimeType = "text/javascript";

        //
        // GET: /WeChatSignature/

        public ActionResult Index(string appId, string url, string debug)
        {
            if(string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(url)){
                return HttpNotFound();
            }

            var all = DataStore.FromConfiguration.Credentials;
            var credential = all.FirstOrDefault(c => c.AppId == appId);
            if (credential == null)
            {
                return HttpNotFound();
            }

            return CreateResponseResult(credential, url, !string.IsNullOrWhiteSpace(debug));
        }

        private ActionResult CreateResponseResult(DataStore.WeChatCredential credential, string url, bool debug)
        {
            try
            {
                var script = WeChatJs.Worker.GenerateBridgeScriptWithSignature(credential.AppId, credential.AppSecret, url, debug);
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
            return Content(WeChatJs.Worker.GenerateBridgeScriptWithSignature(appId, appSecret, url, true), JavaScriptMimeType);
        }
#endif

    }
}
