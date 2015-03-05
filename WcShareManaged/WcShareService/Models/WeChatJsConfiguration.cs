
namespace WcShareService.Models
{
    public class WeChatJsConfiguration
    {
        public bool DebugMode { get; set; }
        public string AppId { get; set; }
        public long Timestamp { get; set; }
        public string NonceString { get; set; }
        public string Signature { get; set; }
    }
}