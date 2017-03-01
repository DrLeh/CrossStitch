using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CrossStitch.Stitch.V1.Stitch
{
    // Message reader to read messages from the Core to the Stitch
    public class ToStitchMessageReader : IDisposable
    {
        private readonly StreamReader _stdin;
        private const int ReadTimeoutMs = 10000;

        public ToStitchMessageReader(Stream stdin)
        {
            _stdin = new StreamReader(stdin);
        }

        public ToStitchMessage ReadMessage(CancellationToken cancellationToken)
        {
            List<string> lines = new List<string>();
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;
                var task = _stdin.ReadLineAsync();
                bool ok = task.Wait(ReadTimeoutMs, cancellationToken);
                if (!ok)
                    continue;
                var s = task.Result;
                if (s.Trim() == "end")
                    break;
                lines.Add(s);
            }

            string buffer = string.Join("\n", lines);
            return JsonConvert.DeserializeObject<ToStitchMessage>(buffer);
        }

        public void Dispose()
        {
            _stdin.Dispose();
        }
    }
}