using Moq;
using NUnit.Framework;
using System;
using System.Web;

namespace WeChatJs.Tests
{
    [TestFixture]
    public class WorkerTests
    {
        const string DummyAppId = "dummy appid";
        const string DummyAppSecret = "dummy appid";
        const string DummyUrl = "http://tempuri.org";


        [Test]
        public void CanSign()
        {
            SetupCustomWeChatService();
            var configurationWithSignature = Worker.Sign(DummyAppId, DummyAppSecret, DummyUrl);

            Assert.AreEqual(configurationWithSignature.AppId, DummyAppId);

            Assert.IsFalse(configurationWithSignature.DebugMode);
            Assert.IsFalse(configurationWithSignature.DontSetupWeChatOnGeneratingScript);
            Assert.IsNotEmpty(configurationWithSignature.NonceString);
            Assert.IsNotEmpty(configurationWithSignature.Signature);

            var timestamp = configurationWithSignature.Timestamp;
            var unixTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;

            Assert.IsTrue(Math.Abs(unixTimestamp - timestamp) < 3);
        }

        [Test]
        public void CanBuildScriptContent()
        {
            SetupCustomWeChatService();
            var scriptWithSignature = Worker.GenerateBridgeScriptWithSignature(DummyAppId, DummyAppSecret, DummyUrl);
            Assert.IsNotEmpty(scriptWithSignature);
        }

        [Test]
        public void CanSignWithCustomServices()
        {
            const string customAccessToken = "custom access token";
            const string customTicket = "custom ticket";

            var getTicketInvoked = false;
            var getAccessTokenInvoked = false;
            var customServiceBuilder = new Mock<Services.IWeChatServices>();
            customServiceBuilder.Setup(s => s.GetTicket(customAccessToken)).Returns(() =>
            {
                getTicketInvoked = true;
                return customTicket;
            });
            customServiceBuilder.Setup(s => s.GetAccessToken(DummyAppId, DummyAppSecret)).Returns(() =>
            {
                getAccessTokenInvoked = true;
                return customAccessToken;
            });


            var httpContext = new HttpContext(new HttpRequest(string.Empty, DummyUrl, string.Empty), new HttpResponse(new System.IO.StringWriter()));
            httpContext.Items[Worker.ContextKey_WeChatService] = customServiceBuilder.Object;
            HttpContext.Current = httpContext;


            var scriptWithSignature = Worker.Sign(DummyAppId, DummyAppSecret, DummyUrl);
            Assert.IsTrue(getTicketInvoked);
            Assert.IsTrue(getAccessTokenInvoked);
        }

        [Test]
        public void CanRetriveUrlFromHttpContextWhenSign()
        {
            const string url = "http://mysite.com/defaultUrl.aspx";
            var httpContext = new HttpContext(new HttpRequest(string.Empty, url, string.Empty), new HttpResponse(new System.IO.StringWriter()));
            SetupCustomWeChatService(httpContext); 

            var signed = Worker.Sign(DummyAppId, DummyAppSecret);
            Assert.AreSame(signed.Url, url);
        }

        [Test]
        public void WillUseSpecifiedUrlWhenPossible()
        {
            const string url = "http://mysite.com/defaultUrl.aspx";
            var httpContext = new HttpContext(new HttpRequest(string.Empty, url, string.Empty), new HttpResponse(new System.IO.StringWriter()));
            SetupCustomWeChatService(httpContext);

            var signed = Worker.Sign(DummyAppId, DummyAppSecret, DummyUrl);
            Assert.IsTrue(new Uri(signed.Url).Equals(new Uri(DummyUrl)));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), MatchType = MessageMatch.Contains, ExpectedMessage = "url")]
        public void ShouldRejectInvalidUrl()
        {
            SetupCustomWeChatService();
            Worker.Sign(DummyAppId, DummyAppSecret, "my custom invalid url string");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase("", "")]
        public void ShouldRejectEmptyAppCredencials(string appId, string appSecret)
        {
            SetupCustomWeChatService();
            Worker.Sign(appId, appSecret, DummyUrl);
        }



        void SetupCustomWeChatService(HttpContext httpContext = null)
        {
            const string customAccessToken = "custom access token";
            const string customTicket = "custom ticket";

            var customServiceBuilder = new Mock<Services.IWeChatServices>();
            customServiceBuilder.Setup(s => s.GetTicket(customAccessToken)).Returns(customTicket);
            customServiceBuilder.Setup(s => s.GetAccessToken(DummyAppId, DummyAppSecret)).Returns(customAccessToken);

            if (httpContext == null)
            {
                httpContext = new HttpContext(new HttpRequest(string.Empty, DummyUrl, string.Empty), new HttpResponse(new System.IO.StringWriter()));
            };

            httpContext.Items[Worker.ContextKey_WeChatService] = customServiceBuilder.Object;
            HttpContext.Current = httpContext;
        }
    }
}
