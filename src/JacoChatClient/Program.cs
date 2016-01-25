using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JacoChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            JacoChatClient client = new JacoChatClient();
            client.Connect(args[0], Convert.ToInt32(args[1]));
            client.MessageRecieved += client_OnMessageRecieved;

            while (true)
                client.SendRaw(Console.ReadLine());
        }

        static void client_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
