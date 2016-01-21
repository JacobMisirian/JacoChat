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
        }

        public static void server_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Handler.Handle(e.Client, e.Message);
        }
    }
}
