using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    public partial class MessageHandler
    {
        public void NickCommand(Client client, string newNick)
        {
            string oldNick = client.NickName;
            MainClass.Server.Clients.Remove(client);
            client.NickName = newNick;
            MainClass.Server.Clients.Add(client);
            if (oldNick != null && oldNick != "")
            {
                foreach (Channel chan in Channels)
                {
                    if (chan.Clients.ContainsKey(oldNick))
                    {
                        SendToChannel(chan.ChannelName, MessageGeneration.GenerateNick(chan.ChannelName, oldNick, newNick));
                        chan.Clients.Remove(oldNick);
                        chan.Clients.Add(newNick, client);
                    }
                }
            }
        }

        public void PrivmsgCommand(Client client, string reciever, string text)
        {
            string message = MessageGeneration.GeneratePRIVMSG(reciever, client.NickName, text);
            if (reciever.StartsWith("#"))
                SendToChannel(reciever, message);
            else
                SendToUser(reciever, message, client);
        }

        public void JoinCommand(Client client, string chanName)
        {
            int pos = channelExists(chanName);
            if (pos == -1)
            {
                Channels.Add(new Channel(chanName));
                pos = Channels.Count - 1;
                Channels[pos].OpUsers.Add(client.NickName, client);
            }
            else if (Channels[pos].Clients.ContainsKey(client.NickName))
            {
                SendToUser(client.NickName, MessageGeneration.GenerateError("Already in channel " + chanName), client);
                return;
            }
            if (Channels[pos].OpUsers.Count == 0)
                Channels[pos].OpUsers.Add(client.NickName, client);
            Channels[pos].Clients.Add(client.NickName, client);
            SendToChannel(Channels[pos].ChannelName, MessageGeneration.GenerateJoin(Channels[pos].ChannelName, client.NickName));
            SendToUser(client.NickName, MessageGeneration.GenerateNames(Channels[pos].ChannelName, Channels[pos]), client);
            SendToUser(client.NickName, MessageGeneration.GenerateTopic(Channels[pos]), client);
            client.Channels.Add(Channels[pos].ChannelName, Channels[pos]);
        }

        public void PartCommand(Client client, string chanName, string reason)
        {
            int pos = channelExists(chanName);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + chanName), client);
            else if (!Channels[pos].Clients.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
            else
            {
                Channels[pos].Clients.Remove(client.NickName);
                client.Channels.Remove(Channels[pos].ChannelName);
                SendToChannel(chanName, MessageGeneration.GeneratePart(Channels[pos].ChannelName, client.NickName, reason));
            }
        }

        public void NamesCommand(Client client, string chanName)
        {
            int pos = channelExists(chanName);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + chanName), client);
            else if (!Channels[pos].Clients.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
            else
                SendToUser(client.NickName, MessageGeneration.GenerateNames(chanName, Channels[pos]), client);
        }

        public void TopicCommand(Client client, string chanName)
        {
            if (client.Channels.ContainsKey(chanName))
                SendToUser(client.NickName, MessageGeneration.GenerateTopic(client.Channels[chanName]), client);
            else
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
        }

        public void TopicCommand(Client client, string chanName, string newTopic)
        {
            if (!client.Channels.ContainsKey(chanName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + chanName), client);
            else
            {
                Channel chan = client.Channels[chanName];
                if (chan.OpUsers.ContainsKey(client.NickName))
                {
                    chan.ChannelTopic = newTopic;
                    SendToChannel(chan.ChannelName, MessageGeneration.GenerateTopic(chan));
                }
                else
                    SendToUser(client.NickName, MessageGeneration.GenerateError("You are not an OP in " + chanName), client);
            }
        }
    }
}