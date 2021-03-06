﻿using System.Linq;
using System.Text;
using CrossStitch.Core.Messages.Backplane;
using CrossStitch.Core.Utility;
using CrossStitch.Core.Utility.Serialization;
using NetMQ;

namespace CrossStitch.Backplane.Zyre
{
    public class NetMqMessageMapper : IMapper<NetMQMessage, ClusterMessage>, IMapper<ClusterMessage, NetMQMessage>
    {
        private readonly ISerializer _serializer;

        public NetMqMessageMapper(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public ClusterMessage Map(NetMQMessage source)
        {
            var envelope = new ClusterMessageBuilder().Build();
            envelope.Header = _serializer.Deserialize<MessageHeader>(source.Pop().Buffer);
            if (envelope.Header.PayloadType == MessagePayloadType.Raw)
                envelope.RawFrames = source.Select(f => f.ToByteArray()).ToList();
            else if (envelope.Header.PayloadType == MessagePayloadType.Object || envelope.Header.PayloadType == MessagePayloadType.InternalObject)
            {
                var bytes = source.FirstOrDefault()?.ToByteArray();
                if (bytes != null)
                    envelope.PayloadObject = _serializer.DeserializeObject(bytes);
            }
            else if (envelope.Header.PayloadType == MessagePayloadType.CommandString)
            {
                envelope.CommandStrings = source
                    .Select(f => f.ToByteArray())
                    .Select(b => Encoding.Unicode.GetString(b))
                    .ToList();
            }
            return envelope;
        }

        public NetMQMessage Map(ClusterMessage envelope)
        {
            var message = new NetMQMessage();
            byte[] headerFrame = _serializer.Serialize(envelope.Header);
            message.Append(headerFrame);
            if (envelope.Header.PayloadType == MessagePayloadType.CommandString)
            {
                foreach (var command in envelope.CommandStrings)
                {
                    var bytes = Encoding.Unicode.GetBytes(command);
                    message.Append(bytes);
                }
            }
            if (envelope.Header.PayloadType == MessagePayloadType.Object || envelope.Header.PayloadType == MessagePayloadType.InternalObject)
            {
                var bytes = _serializer.Serialize(envelope.PayloadObject);
                message.Append(bytes);
            }
            else if (envelope.Header.PayloadType == MessagePayloadType.Raw)
            {
                foreach (var payload in envelope.RawFrames)
                    message.Append(payload);
            }
            return message;
        }
    }
}