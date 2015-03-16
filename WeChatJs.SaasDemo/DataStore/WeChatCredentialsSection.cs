using System.Collections.Generic;
using System.Configuration;

namespace WeChatJs.SaasDemo.DataStore
{
    public class WeChatCredentialsSection: ConfigurationSection
    {
        [ConfigurationProperty("credentials", IsDefaultCollection=false)]
        [ConfigurationCollection(typeof(CredentialCollection))]
        public CredentialCollection Credentials {
            get {
                return this["credentials"] as CredentialCollection;
            }
        }
    }

    public class CredentialCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public WeChatCredential this[int index]
        {
            get { return (WeChatCredential)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public void Add(WeChatCredential element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WeChatCredential();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WeChatCredential)element).AppId;
        }

        public void Remove(WeChatCredential element)
        {
            BaseRemove(element.AppId);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }

    public class WeChatCredential : ConfigurationElement
    {
        [ConfigurationProperty("appId", IsRequired=true)]
        public string AppId {
            get { return base["appId"] as string; }
            set { base["appId"] = value; }
        }

        [ConfigurationProperty("appSecret", IsRequired = true)]
        public string AppSecret{
            get { return base["appSecret"] as string; }
            set { base["appSecret"] = value; }
        }
    }
}