using System;
using System.Collections.Generic;

namespace JacoChatServer
{
    public class MessageGeneration
    {
        public static string GeneratePRIVMSG(string reciever, string user, string message)
        {
            return user + " PRIVMSG " + reciever + " :" + message;
        }

        public static string GenerateError(string message)
        {
            return "server ERROR :" + message;
        }

        public static string GeneratePart(string channel, string user, string reason)
        {
            return user + " PART " + channel + " :" + reason;
        }

        public static string GenerateJoin(string channel, string user)
        {
            return user + " JOIN " + channel;
        }

        public static string GenerateNick(string channel, string oldNick, string newNick)
        {
            return oldNick + " NICK " + channel + " " + newNick;
        }

        public static string GenerateQuit(string channel, string user, string reason)
        {
            return user + " QUIT " + channel + " :" + reason;
        }

        public static string GenerateNames(string channel, Channel chan)
        {
            string nameList = "";
            foreach (KeyValuePair<string, Client> entry in chan.Clients)
                nameList += entry.Value.NickName + " ";
            return channel + " NAMES :" + nameList;
        }

        public static string GenerateTopic(Channel channel)
        {
            return channel.ChannelName + " TOPIC :" + channel.ChannelTopic;
        }

        public static string GenerateWhois(string user, string data)
        {
            return "server WHOIS " + user + " :" + data;
        }
    }
}