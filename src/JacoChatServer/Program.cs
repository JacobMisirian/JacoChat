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
            try
            {
                if (e.Message != null && e != null && e.Message != "")
                    Handler.Handle(e.Client, e.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                e.Client.Send(MessageGeneration.GenerateError("Message not in correct format. " + ex.Message));
            }
        }

        public static void server_OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
            if (e.Client != null && e != null)
            {
                Console.WriteLine(e.Client.NickName + " has disconnected!");
                Server.Clients.Remove(e.Client);
                foreach (Channel channel in Handler.Channels)
                {
                    if (channel.Clients.ContainsValue(e.Client))
                    {
                        channel.Clients.Remove(e.Client.NickName);
                        Handler.SendToChannel(channel, MessageGeneration.GenerateQuit(channel.ChannelName, e.Client.NickName, e.Reason), e.Client);
                    }
                }
            }
        }
    }
}
