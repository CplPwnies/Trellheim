namespace Trellheim.Server.Core.Connectivity
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Data.Client;

    public sealed class AddClient
    {
        public string AccountName { get; private set; }

        public AddClient(string accountName)
        {
            AccountName = accountName;
        }
    }

    public sealed class RemoveClient
    {
        public string AccountName { get; private set; }

        public RemoveClient(string accountName)
        {
            AccountName = accountName;
        }        
    }

    public sealed class IsAuthenticated
    {
        public IsAuthenticated(Account accountInfo, RequestType requestType)
        {
            AccountInfo = accountInfo;
            RequestType = requestType;
        }

        public Account AccountInfo { get; private set; }
        public RequestType RequestType { get; private set; }
    }

    public sealed class AccountAuthenticationStatus
    {
        public AccountAuthenticationStatus(Account accountInfo, RequestType requestType, AuthenticationStatus status)
        {
            AccountInfo = accountInfo;
            RequestType = requestType;
            Status = status;
        }

        public Account AccountInfo { get; private set; }
        public RequestType RequestType { get; private set; }
        public AuthenticationStatus Status { get; private set; }    
    }

    public enum AuthenticationStatus
    {
        NotAuthenticated,
        Authenticated
    }

    public sealed class AuthenticatedConnectionsActor : ReceiveActor
    {
        private readonly IDictionary<string, ActorRef> authenticatedClients = new Dictionary<string, ActorRef>();
        public AuthenticatedConnectionsActor()
        {
            Receive<IsAuthenticated>(msg => Sender.Tell(!authenticatedClients.ContainsKey(msg.AccountInfo.AccountName)
                ? new AccountAuthenticationStatus(msg.AccountInfo, msg.RequestType, AuthenticationStatus.NotAuthenticated)
                : new AccountAuthenticationStatus(msg.AccountInfo, msg.RequestType, AuthenticationStatus.Authenticated)));
            Receive<AddClient>(msg =>
            {
                authenticatedClients[msg.AccountName] = Sender;
            });
            Receive<RemoveClient>(msg =>
            {
                authenticatedClients.Remove(msg.AccountName);
            });
        }
    }
}
