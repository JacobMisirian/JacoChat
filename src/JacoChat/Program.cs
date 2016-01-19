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
                    port = Convert.ToInt32(args[1]);
                    ((JacoChatServer)client).Listen(port);
                    break;
                default:
                    throw new Exception("Must be -c or -l");
            }
            client.MessageRecieved += client_MessageRecieved;

            while (true)
            {
                client.Send(Console.ReadLine());
            }
        }

        private static void client_MessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            Console.WriteLine(e.Bytes + "\t" + e.Message);
        }
    }
}
