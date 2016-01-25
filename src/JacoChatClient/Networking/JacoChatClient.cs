using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JacoChatClient
{
    /// <summary>
    /// Class for connecting, sending, and recieving with a JacoChat Server.
    /// </summary>
    public class JacoChatClient
    {
        private StreamWriter output;
        private StreamReader input;

        /// <summary>
        /// Initializes new JacoChatClient.
        /// </summary>
        public JacoChatClient() { }
        /// <summary>
        /// Initializes new JacoChatClient and connects to the ip and port.
        /// </summary>
        /// <param name="ip">The address to connect to.</param>
        /// <param name="port">The port number.</param>
        public JacoChatClient(string ip, int port)
        {
            Connect(ip, port);
        }

        /// <summary>
        /// Connects to the server on ip and port.
        /// </summary>
        /// <param name="ip">The address to connect to.</param>
        /// <param name="port">The port number.</param>
        public void Connect(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);
            input = new StreamReader(client.GetStream());
            output = new StreamWriter(client.GetStream());

            new Thread(() => beginListening()).Start();
        }

        /// <summary>
        /// Directly writes the message to the output stream.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void SendRaw(string message)
        {
            output.WriteLine(message);
            output.Flush();
        }

        /// <summary>
        /// Sends a privmsg to a channel with a format specifier.
        /// </summary>
        /// <param name="channel">The channel to send to.</param>
        /// <param name="format">The format string.</param>
        /// <param name="args"></param>
        public void SendPrivmsg(string channel, string format, params object[] args)
        {
            SendPrivmsg(channel, string.Format(format, args));
        }
        /// <summary>
        /// Sends a privmsg to a channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="channel">The channel to send to.</param>
        public void SendPrivmsg(string message, string channel)
        {
            SendRaw("PRIVMSG " + channel + " " + message);
        }
        /// <summary>
        /// Joins the channel.
        /// </summary>
        /// <param name="channel">The channel to join.</param>
        public void JoinChannel(string channel)
        {
            SendRaw("JOIN " + channel);
        }
        /// <summary>
        /// Parts from the channel with an option reason.
        /// </summary>
        /// <param name="channel">The channel to part.</param>
        /// <param name="reason">The reason for parting.</param>
        public void PartChannel(string channel, string reason = "")
        {
            SendRaw("PART " + channel + " " + reason);
        }
        /// <summary>
        /// Requests the server to send the topic for the channel.
        /// </summary>
        /// <param name="channel">The channel to get the topic for.</param>
        public void GetTopic(string channel)
        {
            SendRaw("TOPIC " + channel);
        }
        /// <summary>
        /// Sets the topic of a channel (must have ChanOp privileges).
        /// </summary>
        /// <param name="channel">The channel to change.</param>
        /// <param name="topic">The new topic.</param>
        public void SetTopic(string channel, string topic)
        {
            SendRaw("TOPIC " + channel + " " + topic);
        }
        /// <summary>
        /// Requests the server to send back WHOIS data on a user.
        /// </summary>
        /// <param name="user">The user to WHOIS.</param>
        public void Whois(string user)
        {
            SendRaw("WHOIS " + user);
        }
        /// <summary>
        /// Changes your nickname.
        /// </summary>
        /// <param name="newNick">The new nickname.</param>
        public void ChangeNick(string newNick)
        {
            SendRaw("NICK " + newNick);
        }
        /// <summary>
        /// Requests the server to send a names list for a channel.
        /// </summary>
        /// <param name="channel">The channel to get names for.</param>
        public void GetNames(string channel)
        {
            SendRaw("NAMES " + channel);
        }
        /// <summary>
        /// Kicks a user from a channel (must have ChanOp privileges).
        /// </summary>
        /// <param name="user">The user to kick.</param>
        /// <param name="channel">The channel to kick the user from.</param>
        /// <param name="reason">The reason the user is being kicked.</param>
        public void KickUser(string user, string channel, string reason = "")
        {
            SendRaw("KICK " + user + " " + channel + " " + reason);
        }
        /// <summary>
        /// Bans a user from the channel.
        /// </summary>
        /// <param name="user">The user to ban.</param>
        /// <param name="channel">The channel to ban the user from.</param>
        public void BanUser(string user, string channel)
        {
            SendRaw("BAN " + user + " " + channel);
        }
        /// <summary>
        /// Changes the channel op privileges of a user in a channel  (must have ChanOp privileges).
        /// </summary>
        /// <param name="user">The user to change.</param>
        /// <param name="channel">The channel the user is in.</param>
        /// <param name="giveTake">Must be "GIVE" or "TAKE".</param>
        public void ChangeChanOp(string user, string channel, string giveTake)
        {
            SendRaw("CHANOP " + user + " " + channel + " " + giveTake);
        }
        /// <summary>
        /// Requests the server to send a list of channels and information.
        /// </summary>
        public void GetList()
        {
            SendRaw("LIST");
        }

        private void beginListening()
        {
            while (true)
            {
                string responseData = input.ReadLine();
                if (responseData == "PING")
                {
                    output.WriteLine("PONG");
                    output.Flush();
                }
                else
                {
                    OnMessageRecieved(new MessageRecievedEventArgs { Message = responseData });

                    JacoChatMessage msg = JacoChatMessage.Parse(responseData);
                    switch (msg.JacoChatMessageType)
                    {
                        case JacoChatMessageType.BAN:
                            OnBanRecieved(new BanRecievedEventArgs { Channel = msg.Channel, User = msg.Sender });
                            break;
                        case JacoChatMessageType.CHANOP:
                            OnChanOpRecieved(new ChanOpRecievedEventArgs { Channel = msg.Channel, User = msg.Sender });
                            break;
                        case JacoChatMessageType.ERROR:
                            OnErrorRecieved(new ErrorRecievedEventArgs { Error = msg.Body });
                            break;
                        case JacoChatMessageType.JOIN:
                            OnJoinRecieved(new JoinRecievedEventArgs { Channel = msg.Channel, User = msg.Sender });
                            break;
                        case JacoChatMessageType.KICK:
                            OnKickRecieved(new KickRecievedEventArgs { Channel = msg.Channel, User = msg.Sender, Reason = msg.Body });
                            break;
                        case JacoChatMessageType.NAMES:
                            OnNamesRecieved(new NamesRecievedEventArgs { Channel = msg.Channel, List = msg.Body });
                            break;
                        case JacoChatMessageType.NICK:
                            OnNickRecieved(new NickRecievedEventArgs { Channel = msg.Channel, OldNick = msg.Sender, NewNick = msg.Body });
                            break;
                        case JacoChatMessageType.PART:
                            OnPartRecieved(new PartRecievedEventArgs { Channel = msg.Channel, Reason = msg.Body, User = msg.Sender });
                            break;
                        case JacoChatMessageType.PRIVMSG:
                            OnPrivmsgRecieved(new PrivmsgRecievedEventArgs { Channel = msg.Channel, Sender = msg.Sender, Message = msg.Body });
                            break;
                        case JacoChatMessageType.QUIT:
                            OnQuitRecieved(new QuitRecievedEventArgs { Channel = msg.Channel, Reason = msg.Body, User = msg.Sender });
                            break;
                        case JacoChatMessageType.TOPIC:
                            OnTopicRecieved(new TopicRecievedEventArgs { Channel = msg.Channel, Topic = msg.Body });
                            break;
                        case JacoChatMessageType.UNKNOWN:
                            OnUnknownRecieved(new UnknownRecievedEventArgs { Message = msg.Body });
                            break;
                        case JacoChatMessageType.WHOIS:
                            OnWhoisRecieved(new WhoisRecievedEventArgs { User = msg.Sender, Whois = msg.Body });
                            break;
                        case JacoChatMessageType.LIST:
                            OnListRecieved(new ListRecievedEventArgs { Data = msg.Body });
                            break;
                    }
                }
            }
        }

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        protected virtual void OnMessageRecieved(MessageRecievedEventArgs e)
        {
            EventHandler<MessageRecievedEventArgs> handler = MessageRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<PrivmsgRecievedEventArgs> PrivmsgRecieved;
        protected virtual void OnPrivmsgRecieved(PrivmsgRecievedEventArgs e)
        {
            EventHandler<PrivmsgRecievedEventArgs> handler = PrivmsgRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<JoinRecievedEventArgs> JoinRecieved;
        protected virtual void OnJoinRecieved(JoinRecievedEventArgs e)
        {
            EventHandler<JoinRecievedEventArgs> handler = JoinRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<PartRecievedEventArgs> PartRecieved;
        protected virtual void OnPartRecieved(PartRecievedEventArgs e)
        {
            EventHandler<PartRecievedEventArgs> handler = PartRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<QuitRecievedEventArgs> QuitRecieved;
        protected virtual void OnQuitRecieved(QuitRecievedEventArgs e)
        {
            EventHandler<QuitRecievedEventArgs> handler = QuitRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<NickRecievedEventArgs> NickRecieved;
        protected virtual void OnNickRecieved(NickRecievedEventArgs e)
        {
            EventHandler<NickRecievedEventArgs> handler = NickRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<NamesRecievedEventArgs> NamesRecieved;
        protected virtual void OnNamesRecieved(NamesRecievedEventArgs e)
        {
            EventHandler<NamesRecievedEventArgs> handler = NamesRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<TopicRecievedEventArgs> TopicRecieved;
        protected virtual void OnTopicRecieved(TopicRecievedEventArgs e)
        {
            EventHandler<TopicRecievedEventArgs> handler = TopicRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<WhoisRecievedEventArgs> WhoisRecieved;
        protected virtual void OnWhoisRecieved(WhoisRecievedEventArgs e)
        {
            EventHandler<WhoisRecievedEventArgs> handler = WhoisRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<KickRecievedEventArgs> KickRecieved;
        protected virtual void OnKickRecieved(KickRecievedEventArgs e)
        {
            EventHandler<KickRecievedEventArgs> handler = KickRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<BanRecievedEventArgs> BanRecieved;
        protected virtual void OnBanRecieved(BanRecievedEventArgs e)
        {
            EventHandler<BanRecievedEventArgs> handler = BanRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<ChanOpRecievedEventArgs> ChanOpRecieved;
        protected virtual void OnChanOpRecieved(ChanOpRecievedEventArgs e)
        {
            EventHandler<ChanOpRecievedEventArgs> handler = ChanOpRecieved;
            if (handler != null)
                handler(this, e);
        }


        public event EventHandler<ListRecievedEventArgs> ListRecieved;
        protected virtual void OnListRecieved(ListRecievedEventArgs e)
        {
            EventHandler<ListRecievedEventArgs> handler = ListRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<ErrorRecievedEventArgs> ErrorRecieved;
        protected virtual void OnErrorRecieved(ErrorRecievedEventArgs e)
        {
            EventHandler<ErrorRecievedEventArgs> handler = ErrorRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<UnknownRecievedEventArgs> UnknownRecieved;
        protected virtual void OnUnknownRecieved(UnknownRecievedEventArgs e)
        {
            EventHandler<UnknownRecievedEventArgs> handler = UnknownRecieved;
            if (handler != null)
                handler(this, e);
        }
    }
}
