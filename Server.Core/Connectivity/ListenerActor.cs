namespace Trellheim.Server.Core.Connectivity
{
    using System.Net;
    using System.Net.Sockets;
    using Akka.Actor;

    public sealed class IncomingConnection : IOperationResult
    {
        public IncomingConnection(TcpClient client)
        {
            Client = client;
        }

        public TcpClient Client { get; private set; }   
    }

    public sealed class ListenerActor : ReceiveActor
    {
        private readonly TcpListener listener;

        public ListenerActor(IPEndPoint endpoint)
        {
            listener = new TcpListener(endpoint);

            Receive<IncomingConnection>(connection =>
            {
                var client = connection.Client;
                if (client.Connected)
                {
                    Context.System.ActorOf(Props.Create(() => new ClientConnectionActor(client), new OneForOneStrategy(_ => Directive.Stop))) // Always stop on failure. The client will have to reconnect.
                        .Tell(new AuthenticateConnection());
                }
                AcceptConnection();
            });

            Receive<OperationError>(error => error.ExceptionDispatchInfo.Throw());
            listener.Start();
            AcceptConnection();
        }

        private void AcceptConnection()
        {
            listener.AcceptTcpClientAsync().ContinueWith<IOperationResult>(tcp =>
            {
                if (tcp.IsFaulted)
                {
                    return new OperationError(tcp.Exception);
                }
                return new IncomingConnection(tcp.Result);
            }).PipeTo(Self);
        }
    }
}
