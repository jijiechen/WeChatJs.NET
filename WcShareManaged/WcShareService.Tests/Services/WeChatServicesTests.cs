using NUnit.Framework;
using System;
using WcShareService.Providers;
using WcShareService.Services.Impl;
using Moq;
using System.Threading;
using System.Collections.Generic;

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
            service.Setup(wechat, TimeSpan.FromSeconds(0));
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAccessTestByEmptyAppSecret()
        {
            var service = new WeChatService();
            service.GetAccessToken(testAppId, string.Empty);
        }

        [Test]
        public void GetWeChatServiceWithProvider()
        {
            var service = new WeChatService();

            wechat = new WeChatWithApiLimitation(2);
            service.Setup(wechat, TimeSpan.FromSeconds(0));

            var accessToken = service.GetAccessToken("dummy_appid", "dummy_secret");
            Assert.IsTrue(accessToken.StartsWith("access_token_for_dummy_appid"));
            Assert.AreNotSame(accessToken.StartsWith("access_token_for_dummy_appid"), accessToken);

            var jsTicket = service.GetTicket("my_access_token");
            Assert.IsTrue(jsTicket.StartsWith("jsticket_my_access_token"));            
            Assert.AreNotSame(service.GetTicket("my_access_token"), jsTicket);
        }

        [Test]
        [ExpectedException(typeof(WeChatWithApiLimitation.LimitationExceetedException))]
        public void ApiInvokingCouldReachLimitation()
        {
            var service = new WeChatService();

            wechat = new WeChatWithApiLimitation(1);
            service.Setup(wechat, TimeSpan.FromSeconds(0));

            service.GetAccessToken("dummy_appid", "dummy_secret");
            service.GetAccessToken("dummy_appid", "dummy_secret");
        }

        [Test]
        public void ApiInvokingLimitationIsSeperatedOverAppIdsAndAccessTokens()
        {
            var service = new WeChatService();

            wechat = new WeChatWithApiLimitation(1);
            service.Setup(wechat, TimeSpan.FromSeconds(0));

            service.GetAccessToken("dummy_appid", "dummy_secret");
            service.GetAccessToken("another_dummy_appid", "dummy_secret");

            service.GetTicket("my_access_token");
            service.GetTicket("another_my_access_token");
        }

        [Test]
        public void WeChatServiceCanCache()
        {
            var service = new WeChatService();

            wechat = new WeChatWithApiLimitation(1);
            service.Setup(wechat, TimeSpan.FromSeconds(1));
            var accessToken1 = service.GetAccessToken("dummy_appid", "dummy_secret");
            var accessToken2 = service.GetAccessToken("dummy_appid", "dummy_secret");

            Assert.AreEqual(accessToken1, accessToken2);
        }

        [Test]
        [ExpectedException(typeof(WeChatWithApiLimitation.LimitationExceetedException))]
        public void CacheInWeChatServiceCouldExpire()
        {
            var service = new WeChatService();

            wechat = new WeChatWithApiLimitation(1);
            service.Setup(wechat, TimeSpan.FromSeconds(1));
            service.GetAccessToken("dummy_appid", "dummy_secret");

            Thread.Sleep(2000);
            service.GetAccessToken("dummy_appid", "dummy_secret");
        }

        private class WeChatWithApiLimitation : IWeChat
        {
            private Dictionary<string, int> _accessTokenInvocations = new Dictionary<string, int>();
            private Dictionary<string, int> _jsTicketInvocations = new Dictionary<string, int>();
            private static object _locker = new object();
            private static Random _random = new Random();

            private int _limitaion = 0;

            public WeChatWithApiLimitation(int eachApiLimitation)
            {
                _limitaion = eachApiLimitation;
            }

            public string ProvideAccessToken(string appId, string appSecret)
            {
                lock (_locker)
                {
                    int invocationRecordsForAppId;
                    if (!_accessTokenInvocations.TryGetValue(appId, out invocationRecordsForAppId))
                    {
                        invocationRecordsForAppId = _accessTokenInvocations[appId] = 1;
                    }
                    else
                    {
                        invocationRecordsForAppId++;
                    }

                    if (invocationRecordsForAppId > _limitaion)
                    {
                        throw new LimitationExceetedException();
                    }
                }

                return "access_token_for_" + appId + (_random.Next(10000,999999));
            }

            public string ProvideJsTicket(string accessToken)
            {
                lock (_locker)
                {
                    int invocationRecordsForAccessToken;
                    if (!_jsTicketInvocations.TryGetValue(accessToken, out invocationRecordsForAccessToken))
                    {
                        invocationRecordsForAccessToken = _jsTicketInvocations[accessToken] = 1;
                    }
                    else
                    {
                        invocationRecordsForAccessToken++;
                    }

                    if (invocationRecordsForAccessToken > _limitaion)
                    {
                        throw new LimitationExceetedException();
                    }
                }

                return "jsticket_" + accessToken + (_random.Next(10000, 999999));
            }

            public class LimitationExceetedException : Exception { }
        }
    }
}
