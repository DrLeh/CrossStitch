﻿using CrossStitch.Core.Models;
using CrossStitch.Core.Modules.Data;
using CrossStitch.Core.Modules.Logging;
using CrossStitch.Core.Modules.Stitches;
using CrossStitch.Core.Node;
using System;

namespace StitchStart.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = NodeConfiguration.GetDefault();
            using (var node = new CrossStitchCore(config))
            {
                var dataStorage = new InMemoryDataStorage();

                dataStorage.Save(new StitchInstance
                {
                    Name = "StitchStart.Client",
                    Application = "StitchStart",
                    Component = "Client",
                    Version = "1",
                    Adaptor = new InstanceAdaptorDetails
                    {
                        RunMode = InstanceRunModeType.V1Process
                    },
                    FullName = "StitchStart.Client",
                    DirectoryPath = ".",
                    ExecutableName = "StitchStart.Client.exe",
                    State = InstanceStateType.Running,
                    LastHeartbeatReceived = 0
                });

                var data = new DataModule(dataStorage);
                node.AddModule(data);

                var stitchesConfiguration = StitchesConfiguration.GetDefault();
                var stitches = new StitchesModule(stitchesConfiguration);
                node.AddModule(stitches);

                var log = Common.Logging.LogManager.GetLogger("CrossStitch");
                var logging = new LoggingModule(log);
                node.AddModule(logging);

                node.Start();
                Console.ReadKey();
                node.Stop();
            }
        }
    }
}
