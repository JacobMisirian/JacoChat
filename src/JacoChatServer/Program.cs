using System;
using System.Collections.Generic;
using System.IO;

namespace JacoChatServer
{
    class MainClass
    {
        public static JacoChatServer Server = new JacoChatServer();
        public static MessageHandler Handler = new MessageHandler();

        public static JacoChatConfiguration Config;
        private static StreamWriter logWriter { get; set; }

        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Error: Must specify config file. Run JacoChatServer.exe [CONFIG.CONF]");
                return;
            }

            Config = new JacoChatConfigurationReader(args[0]).Read();

            if (Config.OutputMode == OutputMode.FilePath)
                logWriter = new StreamWriter(Config.OutputFilePath);

            Server.Listen(Config.HostIp, Config.Port);
            Server.MessageRecieved += server_OnMessageRecieved;
            Server.UserDisconnected += server_OnUserDisconnected;
            Server.UserConnected += server_OnUserConnected;
        }

        public static void server_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            try
            {
                if (e.Message != null && e != null && e.Message != "")
                {
                    Handler.Handle(e.Client, e.Message);
                    if (e.Client.NickName != null && e.Client.NickName != "")
                        ProcessOutput(e.Client.NickName + ": " + e.Message);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                e.Client.Send(MessageGeneration.GenerateError("Message not in correct format. " + ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                e.Client.Send(MessageGeneration.GenerateError("Message not in correct format. " + ex.Message));
            }
        }

        public static void server_OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
            if (e.Client != null && e != null)
            {
                if (!Server.Clients.Contains(e.Client))
                    return;

                ProcessOutput(e.Client.NickName + " has disconnected!");

                foreach (KeyValuePair<string, Channel> chan in e.Client.Channels)
                {
                    Handler.SendToChannel(chan.Value, MessageGeneration.GenerateQuit(chan.Key, e.Client.NickName, "Ping Timeout: 10 seconds."), e.Client);
                    chan.Value.Clients.Remove(e.Client.NickName);
                    if (chan.Value.OpUsers.ContainsKey(e.Client.NickName))
                        chan.Value.OpUsers.Remove(e.Client.NickName);
                }

                Server.Clients.Remove(e.Client);
            }
        }

        public static void server_OnUserConnected(object sender, UserConnectedEventArgs e)
        {
            if (e.Client != null && e != null && Config.MotdPath != "")
                Handler.MotdCommand(e.Client, Config.MotdPath);
        }

        public static void ProcessOutput(string output)
        {
            if (Config.OutputMode == OutputMode.FilePath)
            {
                logWriter.WriteLine(output);
                logWriter.Flush();
            }
            else
                Console.WriteLine(output);
        }
    }
}
