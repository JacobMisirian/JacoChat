using System;

namespace JacoChatClient
{
    public class JacoChatMessage
    {
        public JacoChatMessageType JacoChatMessageType { get; private set; }
        public string Sender { get; private set; }
        public string Body { get; private set; }
        public string Reciever { get; private set; }

        public JacoChatMessage(JacoChatMessageType type, string sender, string reciever, string body)
        {
            JacoChatMessageType = type;
            Sender = sender;
            Body = body;
            Reciever = reciever;
        }

        public static JacoChatMessage Parse(string message)
        {
            string[] parts = message.Split(' ');
            JacoChatMessageType type;
            string sender = parts[0];
            string reciever = "";
            string body = "";
            switch (parts[1])
            {
                case "PRIVMSG":
                    type = JacoChatMessageType.PRIVMSG;
                    reciever = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "NICK":
                    type = JacoChatMessageType.NICK;
                    reciever = parts[2];
                    body = parts[3];
                    break;
                case "PART":
                    type = JacoChatMessageType.PART;
                    reciever = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "QUIT":
                    type = JacoChatMessageType.QUIT;
                    reciever = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "ERROR":
                    type = JacoChatMessageType.ERROR;
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                default:
                    type = JacoChatMessageType.DEFAULT;
                    body = message;
                    break;
            }

            return new JacoChatMessage(type, sender, reciever, body);
        }
    }

    public enum JacoChatMessageType
    {
        PRIVMSG,
        NICK,
        JOIN,
        NAMES,
        PART,
        QUIT,
        DEFAULT,
        ERROR
    }
}

