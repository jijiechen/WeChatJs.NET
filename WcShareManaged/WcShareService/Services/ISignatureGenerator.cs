
namespace WcShareService.Providers
{
    public interface ISignatureGenerator
    {
        string GenerateSignature(string jsTicket, string url);
    }
}