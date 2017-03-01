﻿using CrossStitch.Core.Data.Entities;
using CrossStitch.Stitch;
using System;

namespace CrossStitch.Core.Modules.Stitches.Adaptors
{
    public class StitchAdaptorFactory
    {
        private readonly IRunningNodeContext _nodeContext;

        public StitchAdaptorFactory(IRunningNodeContext nodeContext)
        {
            _nodeContext = nodeContext;
        }

        public IStitchAdaptor Create(StitchInstance stitchInstance)
        {
            switch (stitchInstance.Adaptor.RunMode)
            {
                //case InstanceRunModeType.AppDomain:
                //    return new AppDomainAppAdaptor(instance, _network);
                case InstanceRunModeType.V1Process:
                    return new V1ProcessStitchAdaptor(stitchInstance, _nodeContext);
            }

            throw new Exception("Run mode not supported");
        }
    }
}