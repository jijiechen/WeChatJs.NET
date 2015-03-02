
using WcShareService.Providers;
namespace WcShareService.Services
{
    public interface IWeChatServices
    {
        void Setup(IWeChat wechat);
        string GetTicket(string appId);
        string GetAccessToken(string appId, string appSecret);
    }
}