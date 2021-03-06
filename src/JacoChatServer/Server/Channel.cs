using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    public class Channel
    {
        public string ChannelName { get; private set; }
        public string ChannelTopic { get; set; }
        public Dictionary<string, Client> Clients { get; private set; }
        public Dictionary<string, Client> OpUsers { get; private set; }
        public Dictionary<string, Client> BannedUsers { get; private set; }

        public Channel(string channelName)
        {
            ChannelName = channelName;
            ChannelTopic = "";
            Clients = new Dictionary<string, Client>();
            OpUsers = new Dictionary<string, Client>();
            BannedUsers = new Dictionary<string, Client>();
        }

        public Channel(string channelName, string channelTopic)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            Clients = new Dictionary<string, Client>();
            OpUsers = new Dictionary<string, Client>();
            BannedUsers = new Dictionary<string, Client>();
        }

        public Channel(string channelName, string channelTopic, Dictionary<string, Client> clients)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            Clients = clients;
            OpUsers = new Dictionary<string, Client>();
            BannedUsers = new Dictionary<string, Client>();
        }

        public Channel(string channelName, string channelTopic, Dictionary<string, Client> clients, Dictionary<string, Client> opUsers)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            Clients = clients;
            OpUsers = opUsers;
            BannedUsers = new Dictionary<string, Client>();
        }
    }
}

