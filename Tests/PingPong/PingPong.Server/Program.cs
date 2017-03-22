﻿using CrossStitch.Core;
using CrossStitch.Core.Models;
using CrossStitch.Core.Modules.Data;
using CrossStitch.Core.Modules.Data.InMemory;
using CrossStitch.Core.Modules.Logging;
using CrossStitch.Core.Modules.Stitches;
using System;
using System.Collections.Generic;

namespace PingPong.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = NodeConfiguration.GetDefault();
            using (var core = new CrossStitchCore(config))
            {
                var dataStorage = new InMemoryDataStorage();

                var ping = new StitchInstance
                {
                    Name = "PingPong.Ping",
                    GroupName = new StitchGroupName("PingPong", "Ping", "1"),
                    Adaptor = new InstanceAdaptorDetails
                    {
                        Type = AdaptorType.ProcessV1,
                        Parameters = new Dictionary<string, string>
                        {
                            { CrossStitch.Stitch.ProcessV1.Parameters.DirectoryPath, "." },
                            { CrossStitch.Stitch.ProcessV1.Parameters.ExecutableName, "PingPong.Ping.exe" }
                        }
                    },
                    State = InstanceStateType.Running,
                    LastHeartbeatReceived = 0
                };
                var pong = new StitchInstance
                {
                    Name = "PingPong.Pong",
                    GroupName = new StitchGroupName("PingPong", "Pong", "1"),
                    Adaptor = new InstanceAdaptorDetails
                    {
                        Type = AdaptorType.ProcessV1,
                        Parameters = new Dictionary<string, string>
                        {
                            { CrossStitch.Stitch.ProcessV1.Parameters.DirectoryPath, "." },
                            { CrossStitch.Stitch.ProcessV1.Parameters.ExecutableName, "PingPong.Pong.exe" }
                        }
                    },
                    State = InstanceStateType.Running,
                    LastHeartbeatReceived = 0
                };

                dataStorage.Save(ping, true);
                dataStorage.Save(pong, true);

                var data = new DataModule(core.MessageBus, dataStorage);
                core.AddModule(data);

                var stitchesConfiguration = StitchesConfiguration.GetDefault();
                var stitches = new StitchesModule(core, stitchesConfiguration);
                core.AddModule(stitches);

                var log = Common.Logging.LogManager.GetLogger("CrossStitch");
                var logging = new LoggingModule(core, log);
                core.AddModule(logging);

                // TODO: We need a way to start for initialization to complete, either having Start
                // block or providing an Initialized event which waits for all modules to report
                // being initialized
                core.Start();

                Console.ReadKey();
                core.Stop();
            }
        }
    }
}