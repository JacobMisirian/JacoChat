﻿using System;

namespace JacoChatClient
{
    /// <summary>
    /// JacoChat message.
    /// </summary>
    public class JacoChatMessage
    {
        /// <summary>
        /// Gets the type of the jaco chat message.
        /// </summary>
        /// <value>The type of the jaco chat message.</value>
        public JacoChatMessageType JacoChatMessageType { get; private set; }
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public string Sender { get; private set; }
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public string Channel { get; private set; }
        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>The body.</value>
        public string Body { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="JacoChatClient.JacoChatMessage"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="channel">Channel.</param>
        /// <param name="body">Body.</param>
        public JacoChatMessage(JacoChatMessageType type, string sender, string channel, string body)
        {
            JacoChatMessageType = type;
            Sender = sender;
            Channel = channel;
            Body = body;
        }
        /// <summary>
        /// Parse the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        public static JacoChatMessage Parse(string message)
        {
            string[] parts = message.Split(' ');
            JacoChatMessageType type;
            string sender;
            string channel;
            string body;

            switch (parts[1].ToUpper())
            {
                case "PRIVMSG":
                    type = JacoChatMessageType.PRIVMSG;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "ERROR":
                    type = JacoChatMessageType.ERROR;
                    sender = parts[0];
                    channel = parts[0];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "PART":
                    type = JacoChatMessageType.PART;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "JOIN":
                    type = JacoChatMessageType.JOIN;
                    sender = parts[0];
                    channel = parts[2];
                    body = "";
                    break;
                case "NICK":
                    type = JacoChatMessageType.NICK;
                    sender = parts[0];
                    channel = parts[2];
                    body = parts[3];
                    break;
                case "QUIT":
                    type = JacoChatMessageType.QUIT;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "NAMES":
                    type = JacoChatMessageType.NAMES;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "TOPIC":
                    type = JacoChatMessageType.TOPIC;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "WHOIS":
                    type = JacoChatMessageType.WHOIS;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "KICK":
                    type = JacoChatMessageType.KICK;
                    sender = parts[0];
                    channel = parts[2];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "BAN":
                    type = JacoChatMessageType.BAN;
                    sender = parts[0];
                    channel = parts[2];
                    body = parts[2];
                    break;
                case "UNBAN":
                    type = JacoChatMessageType.UNBAN;
                    sender = parts[0];
                    channel = parts[2];
                    body = parts[2];
                    break;
                case "CHANOP":
                    type = JacoChatMessageType.CHANOP;
                    sender = parts[3];
                    channel = parts[2];
                    body = parts[4];
                    break;
                case "LIST":
                    type = JacoChatMessageType.LIST;
                    sender = "";
                    channel = "";
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                case "MOTD":
                    type = JacoChatMessageType.MOTD;
                    sender = parts[0];
                    channel = parts[0];
                    body = message.Substring(message.IndexOf(":") + 1);
                    break;
                default:
                    type = JacoChatMessageType.UNKNOWN;
                    sender = "";
                    channel = "";
                    body = message;
                    break;
            }

            return new JacoChatMessage(type, sender, channel, body);
        }
    }

    /// <summary>
    /// Jaco chat message type.
    /// </summary>
    public enum JacoChatMessageType
    {
        PRIVMSG,
        NICK,
        JOIN,
        PART,
        QUIT,
        TOPIC,
        ERROR,
        UNKNOWN,
        NAMES,
        WHOIS,
        KICK,
        BAN,
        UNBAN,
        CHANOP,
        LIST,
        MOTD
    }
}
