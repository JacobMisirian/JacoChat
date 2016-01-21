using System;
using System.Collections.Generic;
using System.Text;

namespace JacoChatServer
{
    public class MessageHandler
    {
        public List<Channel> Channels = new List<Channel>();

        public void Handle(Client client, string text)
        {
            var parts = text.Split(' ');
            string chanName = "";
            string message = "";
            int pos = 0;
            switch (parts[0])
            {
                case "REGISTER":
                    case "NICK":
                    MainClass.Server.Clients.Remove(client);
                    client.NickName = parts[1];
                    MainClass.Server.Clients.Add(client);
                    break;
                    case "PRIVMSG":
                    message = MessageGeneration.GeneratePRIVMSG(parts[1], client.NickName, substringStringArray(parts, 2));
                    if (parts[1].StartsWith("#"))
                        SendToChannel(parts[1], message);
                    else
                        SendToUser(parts[1], message, client);
                    break;
                case "JOIN":
                    chanName = parts[1];
                    pos = channelExists(chanName);
                    if (pos == -1)
                    {
                        Channels.Add(new Channel(chanName));
                        pos = Channels.Count - 1;
                    }
                    Channels[pos].Clients.Add(client);
                    SendToChannel(Channels[pos].ChannelName, MessageGeneration.GenerateJoin(Channels[pos].ChannelName, client.NickName));
                    SendToUser(client.NickName, MessageGeneration.GenerateNames(Channels[pos].ChannelName, Channels[pos]), client);
                    break;
                    case "PART":
                    chanName = parts[1];
                    pos = channelExists(chanName);
                    if (pos == -1)
                    {
                        SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + chanName), client);
                        break;
                    }
                    if (!Channels[pos].Clients.Contains(client))
                    {
                        SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
                        break;
                    }
                    Channels[pos].Clients.Remove(client);
                    SendToChannel(chanName, MessageGeneration.GeneratePart(Channels[pos].ChannelName, client.NickName, substringStringArray(parts, 2)));
                    break;
                    case "NAMES":
                    chanName = parts[1];
                    pos = channelExists(chanName);
                    if (pos == -1)
                    {
                        SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + chanName), client);
                        break;
                    }
                    if (!Channels[pos].Clients.Contains(client))
                    {
                        SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
                        break;
                    }
                    SendToUser(client.NickName, MessageGeneration.GenerateNames(chanName, Channels[pos]), client);
                    break;
            }
        }

        public void SendToChannel(string channelName, string message)
        {
            foreach (Channel channel in Channels)
                if (channel.ChannelName == channelName)
                    foreach (Client client in channel.Clients)
                        client.Send(message);
        }

        public void SendToUser(string user, string message, Client sender)
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
    }
}