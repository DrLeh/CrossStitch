﻿using System.Collections.Generic;
using System.Linq;
using CrossStitch.Core.Messages.Backplane;
using CrossStitch.Core.Models;
using CrossStitch.Core.Modules.Master.Models;

namespace CrossStitch.Core.Modules.Master
{
    public class MasterStitchCache
    {
        private readonly string _nodeId;

        private readonly Dictionary<string, List<StitchSummary>> _remoteStitches;
        private List<StitchSummary> _localStitches;
        private List<StitchSummary> _allStitches;

        public MasterStitchCache(string nodeId, List<StitchSummary> initialLocals, Dictionary<string, List<StitchSummary>> initialRemotes)
        {
            _nodeId = nodeId;
            _localStitches = initialLocals ?? new List<StitchSummary>();
            _remoteStitches = initialRemotes ?? new Dictionary<string, List<StitchSummary>>();
        }

        // These three mutator methods are thread-synchronized by the MasterModule. Only the GetStitchSummaries() method is
        // concurrent.

        public void AddNodeStatus(ReceivedEvent received, NodeStatus status)
        {
            // TODO: Should we enforce ordering? If a node status with an older version comes after one with a newer
            // version, should we reject it?
            if (status.Id == _nodeId)
                return;

            var summaries = status.StitchInstances
                .Where(ii => ii.State == InstanceStateType.Running || ii.State == InstanceStateType.Started)
                .Select(si => new StitchSummary
                {
                    Id = si.Id,
                    GroupName = new StitchGroupName(si.GroupName),
                    Locale = StitchLocaleType.Remote,
                    NetworkNodeId = received.FromNetworkId,
                    NodeId = received.FromNodeId
                })
                .ToList();
            if (!_remoteStitches.ContainsKey(received.FromNodeId))
                _remoteStitches.Add(received.FromNodeId, summaries);
            else
                _remoteStitches[received.FromNodeId] = summaries;
            _allStitches = null;
        }

        public void AddLocalStitch(string id, StitchGroupName groupName)
        {
            var locals = _localStitches
                .Where(si => si.Id != id)
                .Concat(new[]
                {
                    new StitchSummary
                    {
                        Id = id,
                        GroupName = groupName,
                        Locale = StitchLocaleType.Local,
                        NodeId = _nodeId
                    }
                })
                .ToList();
            _localStitches = locals;
            _allStitches = null;
        }

        public void RemoveLocalStitch(string id)
        {
            var locals = _localStitches
                .Where(si => si.Id != id)
                .ToList();
            _localStitches = locals;
            _allStitches = null;
        }

        public List<StitchSummary> GetStitchSummaries()
        {
            var all = _allStitches;
            if (all != null)
                return all;

            var remotes = _remoteStitches.Values.ToList();
            var locals = _localStitches;

            all = remotes.SelectMany(s => s).Concat(locals).ToList();
            _allStitches = all;
            return all;
        }
    }
}