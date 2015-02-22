namespace Trellheim.Data.Client
{
    public sealed class Request : IMessage
    {
        public RequestType RequestType { get; set; }
        public object Payload { get; set; }
    }
}
