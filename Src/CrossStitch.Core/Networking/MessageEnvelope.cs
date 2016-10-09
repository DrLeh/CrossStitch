﻿using System.Collections.Generic;

namespace CrossStitch.Core.Networking
{
    public class MessageEnvelope
    {
        public const string SendEventName = "Send";
        public MessageHeader Header { get; set; }
        public List<string> CommandStrings { get; set; }
        public List<object> PayloadObjects { get; set; }
        public List<byte[]> RawFrames { get; set; }
    }
}