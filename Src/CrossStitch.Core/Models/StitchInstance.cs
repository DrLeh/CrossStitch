﻿using System.Collections.Generic;

namespace CrossStitch.Core.Models
{
    public class StitchInstance : IDataEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long StoreVersion { get; set; }

        public StitchGroupName GroupName { get; set; }

        public InstanceAdaptorDetails Adaptor { get; set; }

        public InstanceStateType State { get; set; }
        public long LastHeartbeatReceived { get; set; }

        public bool IsStartedOrRunning()
        {
            return State == InstanceStateType.Started || State == InstanceStateType.Running;
        }
    }

    public enum InstanceStateType
    {
        // The stitch has started, but hasn't yet checked in
        Started,

        // The stitch is running
        Running,

        // The stitch could not be started
        Error,

        // The stitch has been stopped
        Stopped,

        // The stitch should be running, but it cannot be found.
        Missing
    }

    public enum AdaptorType
    {
        //AppDomain,
        ProcessV1
    }

    public class InstanceAdaptorDetails
    {
        public AdaptorType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
