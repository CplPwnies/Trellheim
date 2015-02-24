namespace Trellheim.Server.Core
{
    using System;
    using System.Net;
    using Akka.Actor;
    using Connectivity;

    public static class Server
    {
        public static void Main()
        {
            var system = ActorSystem.Create("Trellheim");
            system.ActorOf(Props.Create(() => new ListenerActor(new IPEndPoint(IPAddress.Loopback, 7502))), ActorNames.Listener);
            system.ActorOf<AuthenticatedConnectionsActor>(ActorNames.AuthenticatedClients);

            Console.WriteLine("System is running...");
            Console.ReadLine();
        }
    }
}
