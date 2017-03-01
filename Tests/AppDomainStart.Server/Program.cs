﻿using System;
using CrossStitch.Core.Modules.Stitches;
using CrossStitch.Core.Networking.NetMq;

namespace AppDomainStart.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //var instance = new ComponentInstance(Guid.Empty, Guid.Empty) {
            //    DirectoryPath = @"C:\Projects\CrossStitch\Tests\AppDomainStart.Client\bin\Debug",
            //    ExecutableName = "AppDomainStart.Client.exe",
            //    ApplicationClassName = "MyComponent"
            //};
            var network = new NetMqNetwork();
            //var adaptor = new AppDomainAppAdaptor(instance, network);
            //adaptor.AppInitialized += AdaptorOnAppInitialized;
            //bool started = adaptor.Start();
            //Console.WriteLine("Started appDomain: " + started);
            Console.ReadKey();
            //adaptor.Stop();
        }

        private static void AdaptorOnAppInitialized(object sender, StitchStartedEventArgs stitchStartedEventArgs)
        {
            Console.WriteLine("Started app:" + stitchStartedEventArgs.InstanceId);
        }
    }
}
