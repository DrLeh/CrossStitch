﻿using Acquaintance;
using Acquaintance.Timers;
using CrossStitch.Core.MessageBus;
using CrossStitch.Core.Messages;
using CrossStitch.Core.Models;
using System.Linq;

namespace CrossStitch.Core.Modules.Master
{
    // The Master module coordinates multipart-commands across the cluster.
    public class MasterModule : IModule
    {
        private readonly IClusterNodeManager _nodeManager;

        private CrossStitchCore _core;
        private DataHelperClient _data;
        private SubscriptionCollection _subscriptions;
        private IMessageBus _messageBus;
        private ModuleLog _log;

        // TODO: We need to keep track of Backplane zones, so we can know to schedule certain
        // commands only on nodes of certain zones.

        // TODO: We need some kind of scoring metric for a node to report, which will take into 
        // account the number of processor cores and available RAM, and reduce by the number of
        // running stitches, so we can know which nodes to deploy stitches to.

        // TODO: We need to be storing, through the Data Module, state instance of all nodes in the
        // cluster, including the current running node.

        // TODO: We need a model class that can represent the node with status, along with an array
        // of application.component.version/StitchId running on that node. We can pass this model 
        // over the network and store in the data module for usage here.

        /* Commands to support:
         * 1) Create N instances of a Stitch version, with automatic balancing across the cluster to nodes with space
         * 2) Move a stitch instance from current Node to specified remote Node
         * 3) Move a stitch instance from Specified remote node to current node
         * 4) Rebalance instances, by moving stitch instances from an overloaded node to an underloaded node
         * 5) Shutdown stitch instances of a particular version, if A instances are needed but B are running in the cluster and B>A
         * 6) Shutdown all stitch instances of any version besides the current version V, on all nodes.
         * 7) Create A-B stitch instances if A instances are needed but B are running in the cluster and A>B
         * 8) Deploy an application, by Deploying Ni instances of each Stitch/Component version I in an application manifest
         *      The manifest will include specific versions for each component and a number of instances to run for each component version.
         *      
         * Whether we want to represent these things by command objects or by some kind of parsible command script is to be determined.
         */

        // TODO: When we convert an input command to a stream of output commands, we will need some
        // kind of a Job object that we can use to keep track of state. The status of the Job can be
        // queried externally, and the job can be polled regularly to find jobs which are running 
        // very late and need to be alerted about.
        // TODO: A Job should have an ability to be rolled-back, by issuing a sequence of inverse 
        // commands.

        // TODO: Method to lookup NodeId by NetworkNodeId and vice-versa

        //private static bool IsMessageAddressedToAppInstance(MessageEnvelope arg)
        //{
        //    return arg.Header.ToType == TargetType.AppInstance;
        //}

        //private void ResolveAppInstanceNodeIdAndSend(MessageEnvelope obj)
        //{
        //    throw new NotImplementedException();
        //    // TODO: Resolve the NodeId for the message and publish again.
        //}

        public string Name => ModuleNames.Master;

        public void Start(CrossStitchCore core)
        {
            _core = core;
            _messageBus = core.MessageBus;
            _log = new ModuleLog(core.MessageBus, Name);
            _subscriptions = new SubscriptionCollection(_messageBus);
            _data = new DataHelperClient(_messageBus);

            // Publish the status of the node every 60 seconds
            _subscriptions.TimerSubscribe(1, b => b
                .Invoke(t => PublishNodeStatus())
                .OnWorkerThread());

            _subscriptions.TimerSubscribe(1, b => b.Invoke(x => _log.LogDebug("Timer tick")));

            //messageBus.Subscribe<MessageEnvelope>(s => s
            //    .WithChannelName(MessageEnvelope.SendEventName)
            //    .Invoke(ResolveAppInstanceNodeIdAndSend)
            //    .OnWorkerThread()
            //    .WithFilter(IsMessageAddressedToAppInstance)
            //);
        }

        public void Stop()
        {
            if (_core == null)
                return;
            _core = null;
            _nodeManager.Stop();
        }

        public void Dispose()
        {
            Stop();
        }

        private void SendMessageToStitchInstance(string stitchInstanceId, string data)
        {
            // 1) Look through all node statuses for the stitch instance ID
            // 2) create a message for that Node
            // 3) Send the message to the backplane
        }

        private void SendMessageToApplication(string applicationId, string data)
        {
            var application = _data.Get<Application>(applicationId);
            if (application == null)
                return;

            string zone = application.Zone ?? Zones.ZoneAll;
            // TODO: Create a message, addressed to that zone, with the given data.
            // Send to the backplane
        }

        private void SendMessageToStitchesByVersion(string applicationId, string component, string version, string data)
        {
            string fullName = Application.VersionFullName(applicationId, component, version);
            var application = _data.Get<Application>(applicationId);
            if (application == null)
                return;

            string zone = application.Zone ?? Zones.ZoneAll;
            // TODO: Create a message, addressed to that zone, with the given data.
            // Send to the backplane
        }

        private void PublishNodeStatus()
        {
            var modules = _core.AllModules.ToList();
            var stitches = _data.GetAll<StitchInstance>()
                .Where(si => si.State == InstanceStateType.Running || si.State == InstanceStateType.Started)
                .ToList();

            var message = new NodeStatus
            {
                Id = _core.NodeId.ToString(),
                Name = _core.NodeId.ToString(),
                NetworkNodeId = _core.NetworkNodeId,
                RunningModules = modules,
                Instances = stitches
                    .Select(si => new Messages.Stitches.InstanceInformation
                    {
                        Id = si.Id,
                        FullVersionName = si.VersionFullName,
                        State = si.State
                    })
                    .ToList(),

                // This gets enriched in the backplane, for now
                Zones = null
            };

            _data.Save(message, true);
            _messageBus.Publish(NodeStatus.BroadcastEvent, message);
            _log.LogDebug("Published node status");
        }
    }

    //public static class HardwareScoreCalculator
    //{
    //    public static int Calculate()
    //    {
    //        // TODO: Need to get the amount of RAM available, in whole-numbers of GB (rounded) and
    //        // include that in the score somehow.
    //        return Environment.ProcessorCount;
    //    }
    //}
}