﻿namespace CrossStitch.Core.Messages.StitchMonitor
{
    public class StitchHealthRequest
    {
        public string StitchId { get; set; }
    }

    public enum StitchHealthType
    {
        Missing,
        Green,
        Yellow,
        Red
    }

    public class StitchHealthResponse
    {
        public StitchHealthType Status { get; set; }
        public string StitchId { get; set; }

        public static StitchHealthResponse Create(StitchHealthRequest request, StitchHealthType type)
        {
            return new StitchHealthResponse
            {
                Status = type,
                StitchId = request.StitchId
            };
        }

        // TODO: Maybe move this into a proper calculator class
        public static StitchHealthType CalculateHealth(long lastHeartbeatId, long lastSyncId)
        {
            long missedHeartbeats = lastHeartbeatId - lastSyncId;
            if (missedHeartbeats <= 1)
                return StitchHealthType.Green;
            if (missedHeartbeats <= 3)
                return StitchHealthType.Yellow;
            return StitchHealthType.Red;
        }
    }
}
