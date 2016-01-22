using System;
using System.Threading;

namespace JacoChatClient
{
    class MainClass
    {
        public static JacoChatClient Client = new JacoChatClient();

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the server or hostname: ");
            string ip = Console.ReadLine();
            Console.WriteLine("Enter the port: ");
            int port = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the nickname: ");
            string nick = Console.ReadLine();
            Console.WriteLine("Enter the channel: ");
            string channel = Console.ReadLine();

            Client.Connect(ip, port);
            Client.MessageRecieved += Client_OnMessageRecieved;

            Thread.Sleep(1000);

            Client.Send("NICK " + nick);
            Client.Send("JOIN " + channel);

            while (true)
            {
                string input = Console.ReadLine();
                if (input.StartsWith("/"))
                {
                    input = input.Substring(1);
                    string[] parts = input.Split(' ');

                    switch (parts[0])
                    {
                        case "NICK":
                            nick = parts[1];
                            break;
                        case "JOIN":
                            channel = parts[1];
                            break;
                    }

                    Client.Send(input);
                }
                else
                {
                    Client.Send("PRIVMSG " + channel + " " + input);
                }
            }
        }

        public static void Client_OnMessageRecieved(object sender, MessageRecievedEventArgs e)
        {
            JacoChatMessage message = JacoChatMessage.Parse(e.Message);
            switch (message.JacoChatMessageType)
            {
                case JacoChatMessageType.PRIVMSG:
                    Console.WriteLine(message.Channel + " <" + message.Sender + "> " + message.Body);
                    break;
                case JacoChatMessageType.ERROR:
                    Console.WriteLine("<" + message.Sender + "> Error: " + message.Body);
                    break;
                case JacoChatMessageType.JOIN:
                    Console.WriteLine(message.Sender + " has joined " + message.Channel);
                    break;
                case JacoChatMessageType.NAMES:
                    Console.WriteLine("NAMES " + message.Channel + ": " + message.Body);
                    break;
                case JacoChatMessageType.NICK:
                    Console.WriteLine(message.Sender + " has changed nick to " + message.Body);
                    break;
                case JacoChatMessageType.PART:
                    Console.WriteLine(message.Sender + " has left " + message.Channel + " Reason: " + message.Body);
                    break;
                case JacoChatMessageType.QUIT:
                    Console.WriteLine(message.Sender + " has quit " + message.Channel + " Reason: " + message.Body);
                    break;
                case JacoChatMessageType.TOPIC:
                    Console.WriteLine(message.Channel + " topic: " + message.Body);
                    break;
                default:
                    Console.WriteLine(message.Body);
                    break;
            }
        }
    }
}
