using System;
using System.Threading;
using CrossStitch.Core.Messaging.Threading;

namespace CrossStitch.Core.Messaging.RequestResponse
{
    public class DispatchableRequest<TRequest, TResponse> : IThreadAction, IDisposable
    {
        private readonly Func<TRequest, TResponse> _func;
        private readonly TRequest _request;
        private readonly int _timeoutMs;
        private readonly ManualResetEvent _resetEvent;
        public TResponse Response { get; private set; }

        public DispatchableRequest(Func<TRequest, TResponse> func, TRequest request, int timeoutMs = 1000)
        {
            if (timeoutMs <= 0)
                throw new ArgumentOutOfRangeException("timeoutMs");
            _func = func;
            _request = request;
            _timeoutMs = timeoutMs;
            _resetEvent = new ManualResetEvent(false);
            Response = default(TResponse);
        }

        public void Execute(MessageHandlerThreadContext threadContext)
        {
            Response = _func(_request);
            _resetEvent.Set();
        }

        public bool WaitForResponse()
        {
            return _resetEvent.WaitOne(_timeoutMs);
        }

        public void Dispose()
        {
            _resetEvent.Dispose();
        }
    }
}