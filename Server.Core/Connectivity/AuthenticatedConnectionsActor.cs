namespace Trellheim.Server.Core.Connectivity
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Data.Client;

    public sealed class AddClient
    {
        private readonly string accountName;

        public AddClient(string accountName)
        {
            this.accountName = accountName;
        }

        public string AccountName
        {
            get { return accountName; }
        }
    }

    public sealed class RemoveClient
    {
        private readonly string accountName;

        public RemoveClient(string accountName)
        {
            this.accountName = accountName;
        }

        public string AccountName
        {
            get { return accountName; }
        }
    }

    public sealed class IsAuthenticated
    {
        public IsAuthenticated(AccountInfo accountInfo, RequestType requestType)
        {
            AccountInfo = accountInfo;
            RequestType = requestType;
        }

        public AccountInfo AccountInfo { get; private set; }
        public RequestType RequestType { get; private set; }
    }

    public sealed class AccountAuthenticationStatus
    {
        public AccountAuthenticationStatus(AccountInfo accountInfo, RequestType requestType, AuthenticationStatus status)
        {
            AccountInfo = accountInfo;
            RequestType = requestType;
            Status = status;
        }

        public AccountInfo AccountInfo { get; private set; }
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
            Receive<IsAuthenticated>(msg =>
                                     {
                                         Sender.Tell(!authenticatedClients.ContainsKey(msg.AccountInfo.AccountName)
                                             ? new AccountAuthenticationStatus(msg.AccountInfo, msg.RequestType, AuthenticationStatus.NotAuthenticated)
                                             : new AccountAuthenticationStatus(msg.AccountInfo, msg.RequestType, AuthenticationStatus.Authenticated));
                                     });
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
