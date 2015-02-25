namespace Trellheim.Server.Core.Connectivity
{
    using System.IO;
    using System.Net.Sockets;
    using Akka.Actor;
    using Authentication;
    using Newtonsoft.Json;
    using Data.Shared;
    using Request = Data.Client.Request;
    using RequestType = Data.Client.RequestType;
    using Response = Data.Server.Response;
    using ResponseType = Data.Server.ResponseType;

    public sealed class AuthenticateConnection : IOperationResult
    {
    }


    public sealed class ClientConnectionActor : ReceiveActor
    {
        private readonly TcpClient client;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        private readonly ActorRef accountCreation = Context.ActorOf<AccountCreationActor>(ActorNames.AccountCreation);
        private readonly ActorRef accountVerification = Context.ActorOf<AccountVerificationActor>(ActorNames.AccountVerification);
        private readonly ActorSelection clients = Context.ActorSelection(ActorPaths.AuthenticatedClients);

        private string accountName;

        private sealed class UnauthenticatedClientRequest : IOperationResult
        {
            public Request Request { get; private set; }

            public UnauthenticatedClientRequest(Request request)
            {
                Request = request;
            }
        }

        private sealed class AuthenticatedClientRequest : IOperationResult
        {
            public Request Request { get; private set; }

            public AuthenticatedClientRequest(Request request)
            {
                Request = request;
            }
        }

        private sealed class WaitForCommand : IOperationResult { }

        public ClientConnectionActor(TcpClient client)
        {
            this.client = client;
            var stream = this.client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) {AutoFlush = true};
            Receive<OperationError>(msg => msg.ExceptionDispatchInfo.Throw());
            Become(Anonymous);
        }

        protected override void PostStop()
        {
            client.Close();
        }

        private void Stop()
        {
            if (accountName != null)
            {
                clients.Tell(new RemoveClient(accountName));
            }

            Context.System.Stop(Self);
        }

        private void SendResponse(Response response)
        {
            if (client.Connected)
            {
                writer.WriteLine(JsonConvert.SerializeObject(response));
            }
            else
            {
                Stop();
            }
        }

        public void Authenticated()
        {
            Receive<WaitForCommand>(_ =>
            {
                if (client.Connected)
                {
                    reader
                        .ReadLineAsync()
                        .MapAsync(
                            line =>
                                new AuthenticatedClientRequest(
                                    JsonConvert.DeserializeObject<Request>(line)))
                        .PipeTo(Self, Self);
                }
                else
                {
                    Stop();
                }
            });
            Receive<AuthenticatedClientRequest>(msg =>
            {
                var req = msg.Request;
                switch (req.RequestType)
                {
                    case RequestType.Ping:
                        SendResponse(new Response
                        {
                            ResponseType = ResponseType.Ping
                        });
                        break;
                    case RequestType.Disconnect:
                        Stop();
                        return;
                    default:
                        SendResponse(new Response
                        {
                            ResponseType = ResponseType.InvalidCommand
                        });
                        break;
                }
                Self.Tell(new WaitForCommand());
            });
        }

        public void Anonymous()
        {
            Receive<AuthenticateConnection>(msg =>
            {
                if (client.Connected)
                {
                    reader.ReadLineAsync().MapAsync(line => new UnauthenticatedClientRequest(JsonConvert.DeserializeObject<Request>(line)))
                        .PipeTo(Self, Self);
                }
                else
                {
                    Stop();
                }
            });

            Receive<UnauthenticatedClientRequest>(msg =>
            {
                var req = msg.Request;

                switch (req.RequestType)
                {
                    case RequestType.Login:
                    case RequestType.CreateAccount:
                        clients.Ask<AccountAuthenticationStatus>(new IsAuthenticated(req.ToTyped<Account>(), req.RequestType)).PipeTo(Self, Self);
                        break;
                    default:
                        SendResponse(new Response
                        {
                            ResponseType = ResponseType.InvalidCommand,
                            Payload =
                                "Invalid command - client has to be authenticated first via login or account creation"
                        });
                        accountName = null;
                        Become(Anonymous);
                        Self.Tell(new AuthenticateConnection());
                        break;
                }
            });

            Receive<AccountAuthenticationStatus>(msg =>
            {
                switch (msg.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        SendResponse(new Response
                        {
                            ResponseType = ResponseType.AuthenticationFailure,
                            Payload = "Account " + msg.AccountInfo.AccountName + " is already connected and authenticated."
                        });
                        Stop();
                        return;
                    case AuthenticationStatus.NotAuthenticated:
                        switch (msg.RequestType)
                        {
                            case RequestType.Login:
                                accountVerification.Tell(msg.AccountInfo, Self);
                                break;
                            case
                                RequestType.CreateAccount:
                                accountCreation.Tell(msg.AccountInfo, Self);
                                break;
                        }
                        return;
                }
            });

            Receive<AuthenticationSuccessful>(msg =>
            {
                SendResponse(new Response
                {
                    ResponseType = ResponseType.ConnectionAccepted
                });
                accountName = msg.AccountName;
                Become(Authenticated);
                clients.Tell(new AddClient(msg.AccountName));
                Self.Tell(new WaitForCommand());
            });
            Receive<AuthenticationFailed>(msg =>
            {
                SendResponse(new Response
                {
                    ResponseType = ResponseType.AuthenticationFailure,
                    Payload = msg.Message
                });
                accountName = null;
                Become(Anonymous);
                Self.Tell(new AuthenticateConnection());
            });
        }
    }
}
