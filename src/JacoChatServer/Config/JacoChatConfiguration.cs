using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    public class JacoChatConfiguration
    {
        public OutputMode OutputMode { get; set; }
        public string OutputFilePath { get; set; }

        public string HostIp { get; set; }
        public int Port { get; set; }

        public string MotdPath { get; set; }

        public Dictionary<string, string> NetOPs = new Dictionary<string, string>();
    }
    
    public enum OutputMode
    {
        StdOut,
        FilePath
    }
}

