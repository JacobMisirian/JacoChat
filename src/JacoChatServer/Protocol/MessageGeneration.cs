using System;

namespace JacoChatServer
{
    public class MessageGeneration
    {
        public static string GeneratePRIVMSG(string reciever, string user, string message)
        {
            if (reciever.StartsWith("#"))
                return reciever + " PRIVMSG " + user + " :" + message;
            return user + " PRIVMSG " + reciever + " :" + message;
        }

        public static string GenerateError(string message)
        {
            return "server ERROR :" + message;
        }

        public static string GeneratePart(string channel, string user, string reason)
        {
            return channel + " PART " + user + " :" + reason;
        }

        public static string GenerateJoin(string channel, string user)
        {
            return channel + " JOIN " + user;
        }

        public static string GenerateNick(string channel, string oldNick, string newNick)
        {
            return channel + " NICK " + oldNick + " " + newNick;
        }

        public static string GenerateQuit(string channel, string user, string reason)
        {
            return channel + " QUIT " + user + " :" + reason;
        }

        public static string GenerateNames(string channel, Channel chan)
        {
            string nameList = "";
            foreach (Client client in chan.Clients)
                nameList += client.NickName + " ";
            return channel + " NAMES :" + nameList;
        }
    }
}