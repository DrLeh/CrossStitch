﻿using CrossStitch.Stitch.ProcessV1.Core;

namespace CrossStitch.Stitch.BuiltInClassV1
{
    public interface IHandlesHeartbeat
    {
        bool ReceiveHeartbeat(long id);
    }

    public interface IHandlesMessages
    {
        bool ReceiveMessage(long messageId, string channel, string data, string nodeId, string senderStitchInstanceId);
    }

    public interface IHandlesStart
    {
        bool Start(CoreStitchContext context);
    }

    public interface IHandlesStop
    {
        void Stop();
    }
}
