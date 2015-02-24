namespace Trellheim.Server.Core.Authentication
{
    using Akka.Actor;
    using Data.Client;

    public sealed class AccountVerificationActor : ReceiveActor
    {
        public AccountVerificationActor()
        {
            Receive<Account>(msg =>
            {
                // TODO: Verify account
                if (msg.AccountName.Length > 12)
                {
                    Sender.Tell(new AuthenticationFailed("Account name is too long"));
                }
                else
                {
                    Sender.Tell(new AuthenticationSuccessful(msg.AccountName), Self);
                }
            });
        }
    }
}