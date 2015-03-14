using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WeChatJs.Services.Impl
{
    public class SignatureGenerator : ISignatureGenerator
    {
        public WeChatJsConfiguration GenerateWeChatJsConfigurationWithSignature(string jsTicket, string url)
        {
            var jsConfig = new WeChatJsConfiguration { 
                 NonceString = NonceString(),
                 Timestamp = TimeStamp()
            };

            var dataList = new Dictionary<string, string> {
                {"noncestr", jsConfig.NonceString},
                {"timestamp", jsConfig.Timestamp.ToString() },
                {"jsapi_ticket", jsTicket},
                {"url", url}
            }.ToList();

            dataList.Sort(ParameterKeyComparison);
            var queryString = dataList.Aggregate(string.Empty, (query, item) => string.Concat(query, "&", item.Key, "=", item.Value)).TrimStart('&');

            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hashed = sha1.ComputeHash(Encoding.Default.GetBytes(queryString));
                jsConfig.Signature = HexStringFromBytes(hashed);
                return jsConfig;
            }
        }

        static long TimeStamp()
        {
            return (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }

        static string NonceString()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16);
        }

        static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        static int ParameterKeyComparison(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }
}