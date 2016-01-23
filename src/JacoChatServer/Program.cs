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

        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Error: Must specify config file. Run JacoChatServer.exe [CONFIG.CONF]");
                return;
            }

            Config = new JacoChatConfigurationReader(args[0]).Read();

            Server.Listen(Config.HostIp, Config.Port);
            Server.MessageRecieved += server_OnMessageRecieved;
            Server.UserDisconnected += server_OnUserDisconnected;
        }

        public static void server_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            try
            {
                if (e.Message != null && e != null && e.Message != "")
                    Handler.Handle(e.Client, e.Message);
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
                ProcessOutput(e.Client.NickName + " has disconnected!");

                foreach (KeyValuePair<string, Channel> chan in e.Client.Channels)
                {
                    Handler.SendToChannel(chan.Value, MessageGeneration.GenerateQuit(chan.Key, e.Client.NickName, "Ping Timeout: 10 seconds."), e.Client);
                    chan.Value.Clients.Remove(e.Client.NickName);
                    if (chan.Value.OpUsers.ContainsKey(chan.Key))
                        chan.Value.OpUsers.Remove(chan.Key);
                }

              

                Server.Clients.Remove(e.Client);
            }
        }

        public static void ProcessOutput(string output)
        {
            if (Config.OutputMode == OutputMode.FilePath)
                File.AppendAllText(Config.OutputFilePath, output);
            else
                Console.WriteLine(output);
        }
    }
}
