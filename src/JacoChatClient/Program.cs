using System;

namespace JacoChatClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var client = new JacoChatClient();
            client.Connect(args[0], Convert.ToInt32(args[1]));
            client.MessageRecieved += client_MessageRecieved;

            while (true)
                client.Send(Console.ReadLine());
        }

        private static void client_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
           // var messasge = JacoChatMessage.Parse(e.Message);
            //Console.WriteLine("<" + messasge.Sender + ":" + messasge.Reciever + "> " + messasge.Body);
            Console.WriteLine(e.Message);
        }
    }
}
