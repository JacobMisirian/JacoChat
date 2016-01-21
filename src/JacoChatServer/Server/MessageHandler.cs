using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JacoChatServer
{
    public partial class MessageHandler
    {
        public List<Channel> Channels = new List<Channel>();

        public void Handle(Client client, string text)
        {
            var parts = text.Split(' ');
            switch (parts[0])
            {
                case "REGISTER":
                case "NICK":
                    NickCommand(client, parts[1]);
                    break;
                case "PRIVMSG":
                    PrivmsgCommand(client, parts[1], substringStringArray(parts, 2));
                    break;
                case "JOIN":
                    JoinCommand(client, parts[1]);
                    break;
                case "PART":
                    PartCommand(client, parts[1], substringStringArray(parts, 2));
                    break;
                case "NAMES":
                    NamesCommand(client, parts[1]);
                    break;
                case "TOPIC":
                    if (parts.Length >= 3)
                        TopicCommand(client, parts[1], substringStringArray(parts, 2));
                    else
                        TopicCommand(client, parts[1]);
                    break;
            }
        }

        public void SendToChannel(string channelName, string message, Client sender)
        {
            try
            {
                if (!sender.Channels.ContainsKey(channelName))
                    SendToUser(sender.NickName, MessageGeneration.GenerateError("No such channel " + channelName), sender);
                else
                {
                    foreach (KeyValuePair<string, Client> client in sender.Channels[channelName].Clients)
                        SendToUser(client.Value.NickName, message, sender);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendToUser(string user, string message, Client sender)
        {
            try
            {
                foreach (Client client in MainClass.Server.Clients)
                {
                    if (client.NickName == user)
                    {
                        client.Send(message);
                        return;
                    }
                }
                sender.Send(MessageGeneration.GenerateError("No such user " + user));
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private int channelExists(string chanName)
        {
            for (int i = 0; i < Channels.Count; i++)
                if (Channels[i].ChannelName == chanName)
                    return i;
            return -1;
        }

        private string substringStringArray(string[] arr, int startIndex = 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = startIndex; x < arr.Length; x++)
                sb.Append(arr[x] + " ");

            return sb.ToString();
        }

        private bool checkPerms(Channel channel, string nick)
        {
            return channel.OpUsers.ContainsKey(nick);
        }
    }
}