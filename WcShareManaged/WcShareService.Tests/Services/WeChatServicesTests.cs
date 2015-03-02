using NUnit.Framework;
using System;
using WcShareService.Providers;
using WcShareService.Services.Impl;
using Moq;

namespace WcShareService.Tests.Providers
{
    [TestFixture]
    public class AccessTokenProviderTests
    {
        static IWeChat wechat;
        const string testAppId = "some app id";
        const string testAppSecret = "some app id";

        const string testTicket = "this_is_the_ticket";
        const string testAccessToken = "access_by_this_token";

        [SetUp]
        public void MockWeChat()
        {
            var mockWeChat = new Mock<IWeChat>();
            mockWeChat.Setup(w => w.ProvideAccessToken(testAppId, testAppSecret)).Returns( testAccessToken );
            wechat = mockWeChat.Object;
        }


        [Test]
        public void GetAccessTest()
        {
            var service = new WeChatService();
            service.Setup(wechat);
            var accessToken = service.GetAccessToken(testAppId, testAppSecret);
            Assert.AreEqual(accessToken, testAccessToken);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAccessTestByEmptyAppId()
        {
            var service = new WeChatService(); 
            service.GetAccessToken(string.Empty, testAppSecret);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetAccessTestByEmptyAppSecret()
        {
            var service = new WeChatService();
            service.GetAccessToken(testAppId, string.Empty);
        }




    }
}
