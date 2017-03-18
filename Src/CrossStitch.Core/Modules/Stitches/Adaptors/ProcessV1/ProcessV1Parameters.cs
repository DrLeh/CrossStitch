﻿using CrossStitch.Core.Models;
using CrossStitch.Core.Utility.Extensions;
using CrossStitch.Stitch.ProcessV1;
using CrossStitch.Stitch.V1;
using CrossStitch.Stitch.V1.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CrossStitch.Core.Modules.Stitches.Adaptors
{
    public class ProcessV1Parameters
    {
        public ProcessV1Parameters(Dictionary<string, string> parameters)
        {
            DirectoryPath = parameters.GetOrDefault(Parameters.DirectoryPath);
            ExecutableName = parameters.GetOrDefault(Parameters.ExecutableName);
            ExecutableArguments = parameters.GetOrDefault(Parameters.ExecutableArguments);
        }

        public string DirectoryPath { get; set; }
        public string ExecutableName { get; set; }
        public string ExecutableArguments { get; set; }
    }
}