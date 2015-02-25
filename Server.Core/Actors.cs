namespace Trellheim.Server.Core
{
    public static class ActorPaths
    {
        public const string AuthenticatedClients = "/user/" + ActorNames.AuthenticatedClients;
        public const string Listener = "/user/" + ActorNames.Listener;
        public const string Database = "/user/" + ActorNames.Database;
    }

    public static class ActorNames
    {
        public const string AuthenticatedClients = "clients";
        public const string Listener = "listener";
        public const string AccountCreation = "accountCreation";
        public const string AccountVerification = "accountVerification";
        public const string Database = "database";
    }
}

