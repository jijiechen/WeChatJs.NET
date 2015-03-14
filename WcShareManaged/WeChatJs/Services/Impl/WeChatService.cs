using System;
using System.Collections.Concurrent;
using WeChatJs.Providers;
using WeChatJs.Utils;

namespace WeChatJs.Services.Impl
{
    public class WeChatService: IWeChatServices
    {
        private TimeSpan _cacheDuration;
        private IWeChat _wechat;

        public void Setup(IWeChat wechat, TimeSpan cacheDuration)
        {
            if (wechat == null)
            {
                throw new ArgumentNullException("wechat");
            }

            _wechat = wechat;
            _cacheDuration = cacheDuration;
        }

        public string GetTicket(string accessToken)
        {
            Parameters.RequireNotEmpty("accessToken", accessToken); 

            Tuple<string, DateTime> cachedTicketItem;
            if (_cachedAccessTokens.TryGetValue(accessToken, out cachedTicketItem) && CacheNotExpired(cachedTicketItem.Item2))
            {
                return cachedTicketItem.Item1;
            }

            var newTicket = _wechat.ProvideJsTicket(accessToken);
            var newItem = Tuple.Create(newTicket, DateTime.UtcNow);
            _cachedAccessTokens.AddOrUpdate(accessToken, newItem, (key, old) => newItem);
            return newTicket;
        }

        public string GetAccessToken(string appId, string appSecret)
        {
            Parameters.RequireNotEmpty("appId", appId);
            Parameters.RequireNotEmpty("appSecret", appSecret);

            var cachedKey = string.Format("appId={0}&appSecret={1}", appId, appSecret);
            Tuple<string, DateTime> cachedAccessTokenItem;
            if (_cachedAccessTokens.TryGetValue(cachedKey, out cachedAccessTokenItem) && CacheNotExpired(cachedAccessTokenItem.Item2))
            {
                return cachedAccessTokenItem.Item1;
            }

            var newAccessToken = _wechat.ProvideAccessToken(appId, appSecret);
            var newItem = Tuple.Create(newAccessToken, DateTime.UtcNow);
            _cachedAccessTokens.AddOrUpdate(cachedKey, newItem, (key, old) => newItem);

            return newAccessToken;
        }

        private bool CacheNotExpired(DateTime timeCached)
        {
            var dueTime = DateTime.UtcNow - timeCached;
            return dueTime < _cacheDuration;
        }


        private ConcurrentDictionary<string, Tuple<string, DateTime>> _cachedAccessTokens = new ConcurrentDictionary<string, Tuple<string, DateTime>>();
        private ConcurrentDictionary<string, Tuple<string, DateTime>> _cachedJsTickets = new ConcurrentDictionary<string, Tuple<string, DateTime>>();
    }
}