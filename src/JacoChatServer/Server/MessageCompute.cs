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
                foreach (KeyValuePair<string, Channel> chan in client.Channels)
                {
                    SendToChannel(chan.Value, MessageGeneration.GenerateNick(chan.Key, oldNick, newNick), client);
                    chan.Value.Clients.Remove(oldNick);
                    chan.Value.Clients.Add(newNick, client);
                }
            }
        }

        public void PrivmsgCommand(Client client, string reciever, string text)
        {
            string message = MessageGeneration.GeneratePRIVMSG(reciever, client.NickName, text);
            if (reciever.StartsWith("#"))
            {
                int pos = channelExists(reciever);
                if (pos == -1)
                    SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + reciever), client);
                else
                    SendToChannel(Channels[pos], MessageGeneration.GeneratePRIVMSG(reciever, client.NickName, text), client);
            }
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
            client.Channels.Add(Channels[pos].ChannelName, Channels[pos]);
            SendToChannel(Channels[pos], MessageGeneration.GenerateJoin(Channels[pos].ChannelName, client.NickName), client);
            SendToUser(client.NickName, MessageGeneration.GenerateNames(Channels[pos].ChannelName, Channels[pos]), client);
            SendToUser(client.NickName, MessageGeneration.GenerateTopic(Channels[pos]), client);
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
                SendToChannel(Channels[pos], MessageGeneration.GeneratePart(Channels[pos].ChannelName, client.NickName, reason), client);
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
                    SendToChannel(chan, MessageGeneration.GenerateTopic(chan), client);
                }
                else
                    SendToUser(client.NickName, MessageGeneration.GenerateError("You are not an OP in " + chanName), client);
            }
        }
    }
}