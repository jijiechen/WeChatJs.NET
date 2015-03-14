using System.Web;
using System.Web.Mvc;
using WeChatJs;

namespace WcShareService.Controllers
{
    public class WeChatSignatureController : Controller
    {
        //
        // GET: /WeChatSignature/

        public ActionResult Index(string appId, string appSecret, string url)
        {
            //Parameters.RequireNotEmpty("appId", appId);
            //Parameters.RequireNotEmpty("appSecret", appSecret);

            return Content(WeChatJs.Worker.GenerateBridgeScriptWithSignature(appId, appSecret, url),"text/javascript");
        }

    }
}
