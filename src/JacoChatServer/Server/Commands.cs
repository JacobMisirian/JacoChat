using System;
using System.Collections.Generic;
using System.IO;

namespace JacoChatServer
{
    public partial class MessageHandler
    {
        public void NickCommand(Client client, string newNick)
        {
            foreach (Client cli in MainClass.Server.Clients)
            {
                if (cli.NickName == newNick)
                {
                    client.Send(MessageGeneration.GenerateError("Nick " + newNick + " is already taken"));
                    return;
                }
            }

            string oldNick = client.NickName;
            MainClass.Server.Clients.Remove(client);
            client.NickName = newNick;
            MainClass.Server.Clients.Add(client);

            if (oldNick != null && oldNick != "")
            {
                foreach (KeyValuePair<string, Channel> chan in client.Channels)
                {
                    SendToChannel(chan.Value, MessageGeneration.GenerateNick(chan.Key, oldNick, newNick), client, true);
                    chan.Value.Clients.Remove(oldNick);
                    chan.Value.Clients.Add(newNick, client);
                    if (chan.Value.OpUsers.ContainsKey(oldNick))
                    {
                        chan.Value.OpUsers.Remove(oldNick);
                        chan.Value.OpUsers.Add(newNick, client);
                    }
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

            if (!chanName.StartsWith("#"))
            {
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not valid channel " + chanName), client);
                return;
            }

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

            SendToChannel(Channels[pos], MessageGeneration.GenerateJoin(Channels[pos].ChannelName, client.NickName), client, true);
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
                removeUser(client, Channels[pos]);
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
                if (checkPerms(chan, client.NickName))
                {
                    chan.ChannelTopic = newTopic;
                    SendToChannel(chan, MessageGeneration.GenerateTopic(chan), client, true);
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
                SendToChannel(Channels[pos], MessageGeneration.GenerateKick(user, channel, reason), client, true);
                removeUser(requestedClient, Channels[pos]);
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

        public void UnBanCommand(Client client, string user, string channel)
        {
            int pos = channelExists(channel);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + channel), client);
            else if (!Channels[pos].OpUsers.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not an OP in " + channel), client);
            else if (!Channels[pos].BannedUsers.ContainsKey(user))
                SendToUser(client.NickName, MessageGeneration.GenerateError(user + " us already unbanned in " + channel), client);
            else
            {
                SendToChannel(Channels[pos], MessageGeneration.GenerateUnBan(user, channel), client);
                Channels[pos].BannedUsers.Remove(user);
            }
        }

        public void ListCommand(Client client)
        {
            foreach (Channel chan in Channels)
                SendToUser(client.NickName, "server LIST :" + chan.ChannelName + " " + chan.OpUsers.Count + " Ops. " + chan.Clients.Count + " Users. " + chan.ChannelTopic, client);
        }

        public void ChanOpCommand(Client client, string channel, string user, string arg)
        {
            arg = arg.ToUpper();
            if (arg != "GIVE" && arg != "TAKE")
            {
                SendToUser(client.NickName, MessageGeneration.GenerateError("Must supply GIVE or TAKE"), client);
                return;
            }

            int pos = channelExists(channel);
            if (pos == -1)
                SendToUser(client.NickName, MessageGeneration.GenerateError("No such channel " + channel), client);
            else if (!checkPerms(Channels[pos], client.NickName) && !client.NetOp)
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not an OP in " + channel), client);
            else if (checkPerms(Channels[pos], user) && arg != "TAKE")
                SendToUser(client.NickName, MessageGeneration.GenerateError(user + " is already an OP in " + channel), client);
            else if (!checkPerms(Channels[pos], user) && arg != "GIVE")
                SendToUser(client.NickName, MessageGeneration.GenerateError(user + " is not an OP in " + channel), client);
            else if (!Channels[pos].Clients.ContainsKey(client.NickName) || !Channels[pos].Clients.ContainsKey(user))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not in channel " + channel), client);
            else
            {
                SendToChannel(Channels[pos], MessageGeneration.GenerateChanOp(user, channel, arg, client.NickName), client, true);
                if (arg == "GIVE")
                    Channels[pos].OpUsers.Add(user, Channels[pos].Clients[user]);
                else
                    Channels[pos].OpUsers.Remove(user);
            }
        }

        public void NetOpCommand(Client client, string password)
        {
            if (!MainClass.Config.NetOPs.ContainsKey(client.NickName))
                SendToUser(client.NickName, MessageGeneration.GenerateError("Not on the NetOp list."), client);
            else if (client.NetOp)
                SendToUser(client.NickName, MessageGeneration.GenerateError("Already a NetOp"), client);
            else if (MainClass.Config.NetOPs[client.NickName] != password)
                SendToUser(client.NickName, MessageGeneration.GenerateError("Incorrect password for NetOp"), client);
            else
                client.NetOp = true;
        }

        public void MotdCommand(Client client, string motdPath)
        {
            foreach (string line in File.ReadAllLines(motdPath))
                client.Send(MessageGeneration.GenerateMotd(line));
        }

        private void removeUser(Client client, Channel channel)
        {
            channel.Clients.Remove(client.NickName);
            if (channel.OpUsers.ContainsKey(client.NickName))
                channel.OpUsers.Remove(client.NickName);
            client.Channels.Remove(channel.ChannelName);
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