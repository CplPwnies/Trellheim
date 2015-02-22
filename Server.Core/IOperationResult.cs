namespace Trellheim.Server.Core
{
    using System;
    using System.Runtime.ExceptionServices;

    public interface IOperationResult
    {
    }

    public sealed class EmptyResult : IOperationResult { }

    public static class OperationResult
    {
        private static readonly Lazy<EmptyResult> EmptyResult = new Lazy<EmptyResult>(() => new EmptyResult());

        public static EmptyResult Empty { get { return EmptyResult.Value; } }
    }

    public class OperationError : IOperationResult
    {
        public OperationError(Exception exception)
        {
            ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
        }

        public ExceptionDispatchInfo ExceptionDispatchInfo { get; private set; }
    }
}
