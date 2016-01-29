using System;
using System.IO;

namespace JacoChatServer
{
    public class JacoChatConfigurationReader
    {
        private string[] contents { get; set; }

        public JacoChatConfigurationReader(string[] contents)
        {
            this.contents = contents;
        }

        public JacoChatConfigurationReader(string filePath)
        {
            contents = File.ReadAllLines(filePath);
        }

        public JacoChatConfiguration Read()
        {
            JacoChatConfiguration config = new JacoChatConfiguration();
            config.OutputMode = OutputMode.StdOut;
            config.HostIp = "127.0.0.1";
            config.Port = 1337;
            config.MotdPath = "";

            for (int i = 0; i < contents.Length; i++)
            {
                string line = contents[i];
                if (line.Trim().StartsWith("#") || line == "" || line == "\n")
                    continue;
                string[] parts = line.Split(' ');

                switch (parts[0])
                {
                    case "OutputMode":
                        if (parts[1] == "stdout")
                            config.OutputMode = OutputMode.StdOut;
                        else
                        {
                            config.OutputMode = OutputMode.FilePath;
                            config.OutputFilePath = parts[1];
                        }
                        break;
                    case "HostName":
                        config.HostIp = parts[1];
                        break;
                    case "Port":
                        config.Port = Convert.ToInt32(parts[1]);
                        break;
                    case "Motd":
                        config.MotdPath = parts[1];
                        break;
                }
            }
            return config;
        }
    }
}

