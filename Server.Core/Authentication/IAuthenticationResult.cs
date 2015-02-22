namespace Trellheim.Server.Core.Authentication
{
    public interface IAuthenticationResult
    {
    }

    public sealed class AuthenticationFailed : IAuthenticationResult
    {
        public AuthenticationFailed(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public sealed class AuthenticationSuccessful : IAuthenticationResult
    {
        public AuthenticationSuccessful(string accountName)
        {
            AccountName = accountName;
        }

        public string AccountName { get; private set; }
    }
}