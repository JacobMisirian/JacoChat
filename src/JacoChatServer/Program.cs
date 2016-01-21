using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    class MainClass
    {
        public static JacoChatServer Server = new JacoChatServer();
        public static MessageHandler Handler = new MessageHandler();

        public static void Main(string[] args)
        {
            Server.Listen(args[0], Convert.ToInt32(args[1]));
            Server.MessageRecieved += server_OnMessageRecieved;
            Server.UserDisconnected += server_OnUserDisconnected;
        }

        public static void server_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            if (e.Message != null && e != null && e.Message != "")
                Handler.Handle(e.Client, e.Message);
        }

        public static void server_OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
            if (e.Client != null && e != null)
            {
                Console.WriteLine(e.Client.NickName + " has disconnected!");
                Server.Clients.Remove(e.Client);
                foreach (Channel channel in Handler.Channels)
                {
                    if (channel.Clients.Contains(e.Client))
                    {
                        channel.Clients.Remove(e.Client);
                        Handler.SendToChannel(channel.ChannelName, MessageGeneration.GenerateQuit(channel.ChannelName, e.Client.NickName, e.Reason));
                    }
                }
            }
        }
    }
}
