
namespace WcShareService.Providers
{
    public interface IWeChat
    {
        // int validDueTime /* 一个 access_token/ticket 的有效时长 */, int apiLimitation
        string ProvideAccessToken(string appId, string appSecret);
        string ProvideJsTicket(string accessToken);
    }
}
