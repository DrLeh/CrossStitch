﻿using System;
using System.Collections.Generic;

namespace CrossStitch.Core.Modules.Stitches.Versions
{
    public class DateTimeVersionManager : IVersionManager
    {
        public string GetNextAvailableVersion(IEnumerable<string> versions)
        {
            var versionLookup = new HashSet<string>(versions);
            string versionBase = DateTime.UtcNow.ToString("yyyyMMdd_HHmm");
            string version = versionBase;
            int suffix = 0;
            while (versionLookup.Contains(version))
            {
                suffix++;
                version = versionBase + "_" + suffix;
            }
            return version;
        }
    }
}