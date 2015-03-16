
namespace WeChatJs.Services
{
    public interface ISignatureGenerator
    {
        WeChatJsConfiguration GenerateWeChatJsConfigurationWithSignature(string jsTicket, string url);
    }
}