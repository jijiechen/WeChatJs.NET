using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace WeChatJs.SaasDemo.DataStore
{
    public static class FromConfiguration
    {
        private static List<WeChatCredential> _credentials;
        private static object _locker = new object();

        public static ICollection<WeChatCredential> Credentials
        {
            get {
                if (_credentials == null)
                {
                    lock (_locker) {
                        if (_credentials == null) {
                            _credentials = LoadCredentialsFromWebConfiguration();
                        }
                    }
                }

                return _credentials.AsReadOnly();
            }
        }

        private static List<WeChatCredential> LoadCredentialsFromWebConfiguration()
        {
            var section = ConfigurationManager.GetSection("weChatCredentials") as WeChatCredentialsSection;
            if (section == null)
            {
                return new List<WeChatCredential>();
            }

            return section.Credentials.Cast<WeChatCredential>().ToList();
        }
    }
}