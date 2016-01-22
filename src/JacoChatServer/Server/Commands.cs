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
            if (checkForBan(client, reciever, "send to"))
                return;

            string message = MessageGeneration.GeneratePRIVMSG(reciever, client.NickName, text);
            if (reciever.StartsWith("#"))
            {
                int pos = channelExists(reciever);
                if (pos == -1)
                    SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + reciever), client);
                else if (!client.Channels.ContainsKey(reciever))
                    SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + reciever), client);
                else
                    SendToChannel(Channels[pos], MessageGeneration.GeneratePRIVMSG(reciever, client.NickName, text), client);
            }
            else
                SendToUser(reciever, message, client);
        }

        public void JoinCommand(Client client, string chanName)
        {
            if (checkForBan(client, chanName, "join"))
                return;

            int pos = channelExists(chanName);
            if (pos == -1)
            {
                Channels.Add(new Channel(chanName));
                pos = Channels.Count - 1;
                Channels[pos].OpUsers.Add(client.NickName, client);
            }
            else if (Channels[pos].Clients.ContainsKey(client.NickName) || client.Channels.ContainsKey(chanName))
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
                if (Channels[pos].OpUsers.ContainsKey(client.NickName))
                    Channels[pos].OpUsers.Remove(client.NickName);
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
                    SendToUser(client.NickName, MessageGeneration.GenerateError("Not an OP in " + chanName), client);
            }
        }

        public void WhoisCommand(Client client, string user)
        {
            Client requestedClient = null;
            foreach (Client cli in MainClass.Server.Clients)
                if (cli.NickName == user)
                    requestedClient = cli;
            if (requestedClient == null)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such user " + user), client);
            else
            {
                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, requestedClient.NickName), client);
                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, "From: " + requestedClient.IpAddress), client);
                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, "Ping: " + requestedClient.Ping), client);
                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, "Time: " + requestedClient.Time.Elapsed.ToString()), client);
                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, "Idle: " + requestedClient.Idle), client);

                string channelList = "";
                foreach (KeyValuePair<string, Channel> chan in requestedClient.Channels)
                    channelList += chan.Key + " ";

                SendToUser(client.NickName, MessageGeneration.GenerateWhois(user, "Channels: " + channelList), client);
            }
        }

        public void KickCommand(Client client, string user, string channel, string reason)
        {
            int pos = channelExists(channel);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + channel), client);
            else if (!Channels[pos].Clients.ContainsKey(user))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + channel), client);
            else if (!Channels[pos].OpUsers.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not an OP in " + channel), client);
            else
            {
                Client requestedClient = Channels[pos].Clients[user];
                SendToChannel(Channels[pos], MessageGeneration.GenerateKick(user, channel, reason), client);
                Channels[pos].Clients.Remove(user);
                requestedClient.Channels.Remove(channel);
            }
        }

        public void BanCommand(Client client, string user, string channel)
        {
            int pos = channelExists(channel);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + channel), client);
            else if (!Channels[pos].OpUsers.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not an OP in " + channel), client);
            else if (Channels[pos].BannedUsers.ContainsKey(user))
                SendToUser(client.NickName, MessageGeneration.GenerateError(user + " is already banned in " + channel), client);
            else
            {
                SendToChannel(Channels[pos], MessageGeneration.GenerateBan(user, channel), client);
                Channels[pos].BannedUsers.Add(user, Channels[pos].Clients[user]);
            }
        }

        private bool checkForBan(Client client, string channel, string action)
        {
            int pos = channelExists(channel);
            if (pos == -1)
                return false;

            if (Channels[pos].BannedUsers.ContainsKey(client.NickName))
            {
                SendToUser(client.NickName, MessageGeneration.GenerateError("Cannot " + action + " channel. Banned"), client);
                return true;
            }

            return false;
        }
    }
}