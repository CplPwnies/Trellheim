namespace Trellheim.Server.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using Data.Client;

    public static class JsonExtensions
    {
        public static T ToTyped<T>(this Request request)
        {
            return JObject.FromObject(request.Payload).ToObject<T>();
        }
    }

    public static class StreamExtensions
    {
        public static StreamReader CreateReader(this Stream stream)
        {
            return new StreamReader(stream);
        }
        public static StreamWriter CreateWriter(this Stream stream)
        {
            return new StreamWriter(stream);
        }
    }

    public static class TaskExtensions
    {
        public static Task<IOperationResult> MapAsync<TSource, TTarget>(this Task<TSource> task,
            Func<TSource, TTarget> mapper)
            where TTarget : IOperationResult
        {
            return task.ContinueWith<IOperationResult>(result =>
                                                            {
                                                                if (result.IsCanceled || result.IsFaulted)
                                                                {
                                                                    return new OperationError(result.Exception);
                                                                }

                                                                try
                                                                {
                                                                    return mapper(result.Result);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    return new OperationError(ex);
                                                                }
                                                            });
        }

        public static Task<IOperationResult> ExecuteAsync(this Task task, Action action = null)
        {
            return task.ContinueWith<IOperationResult>(result =>
                                                            {
                                                                if (result.IsCanceled || result.IsFaulted)
                                                                {
                                                                    return new OperationError(result.Exception);
                                                                }

                                                                try
                                                                {
                                                                    if (action != null)
                                                                    {
                                                                        action();
                                                                    }
                                                                    return OperationResult.Empty;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    return new OperationError(ex);
                                                                }
                                                            });
        }

        public static Task<IOperationResult> MapAsync<T>(this Task task, Func<T> func)
            where T : IOperationResult
        {
            return task.ContinueWith<IOperationResult>(result =>
                                                            {
                                                                if (result.IsCanceled || result.IsFaulted)
                                                                {
                                                                    return new OperationError(result.Exception);
                                                                }

                                                                try
                                                                {
                                                                    return func();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    return new OperationError(ex);
                                                                }
                                                            });
        }
    }
}
