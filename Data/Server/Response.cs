namespace Trellheim.Data.Server
{
    public sealed class Response : IMessage
    {
        public ResponseType ResponseType { get; set; }
        public object Payload { get; set; }
    }
}