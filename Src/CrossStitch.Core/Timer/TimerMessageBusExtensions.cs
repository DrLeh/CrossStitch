﻿using System;
using CrossStitch.Core.Messaging;

namespace CrossStitch.Core.Timer
{
    public static class TimerMessageBusExtensions
    {
        public static IDisposable TimerSubscribe(this ISubscribable messageBus, int multiple, Action<MessageTimerEvent> subscriber, PublishOptions options = null)
        {
            return messageBus.Subscribe(MessageTimerEvent.EventName, subscriber, t => t.Id % multiple == 0, options);
        }
    }
}
