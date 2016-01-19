using System;

namespace JacoChat
{
    class MainClass
    {
        private static string ip = "";
        private static int port = 0;

        public static void Main(string[] args)
        {
            IJacoChat client;

            switch (args[0])
            {
                case "-c":
                    client = new JacoChatClient();
                    ip = args[1];
                    port = Convert.ToInt32(args[2]);
                    ((JacoChatClient)client).Connect(ip, port);
                    break;
                case "-l":
                    client = new JacoChatServer();
                    ip = args[1];
                    port = Convert.ToInt32(args[2]);
                    ((JacoChatServer)client).Listen(ip, port);
                    break;
                default:
                    throw new Exception("Must be -c or -l");
            }
            client.MessageRecieved += client_MessageRecieved;
            client.UserJoined += client_UserJoined;
            while (true)
                client.Send(Console.ReadLine());
        }

        private static void client_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Console.WriteLine(e.Bytes + "\t" + e.Message);
        }

        private static void client_UserJoined(object sender, UserJoinedEventArgs e)
        {
            Console.WriteLine("A user has connected " + e.Client.IpAddress);
        }
    }
}
