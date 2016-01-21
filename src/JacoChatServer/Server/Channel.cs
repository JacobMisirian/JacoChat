using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    public class Channel
    {
        public string ChannelName { get; private set; }
        public string ChannelTopic { get; set; }
        public List<Client> Clients { get; private set; }

        public Channel(string channelName)
        {
            ChannelName = channelName;
            ChannelTopic = "";
            Clients = new List<Client>();
        }

        public Channel(string channelName, string channelTopic)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            Clients = new List<Client>();
        }

        public Channel(string channelName, string channelTopic, List<Client> clients)
        {
            ChannelName = channelName;
            ChannelTopic = channelTopic;
            Clients = clients;
       }
    }
}

