namespace Trellheim.Server.Core.Authentication
{
    using Akka.Actor;
    using Data.Client;

    public sealed class AccountCreationActor : ReceiveActor
    {
        public AccountCreationActor()
        {
            Receive<Account>(msg =>
            {
                if (msg.AccountName.Length > 20)
                {
                    Sender.Tell(new AuthenticationFailed("Account name is too long."), Self);
                }
                // TODO: Create account
                Sender.Tell(new AuthenticationSuccessful(msg.AccountName), Self);
            });
        }
    }
}