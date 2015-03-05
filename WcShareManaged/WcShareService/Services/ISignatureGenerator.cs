
using WcShareService.Models;
namespace WcShareService.Services
{
    public interface ISignatureGenerator
    {
        WeChatJsConfiguration GenerateWeChatJsConfigurationWithSignature(string jsTicket, string url);
    }
}