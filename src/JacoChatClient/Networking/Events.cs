using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JacoChatClient
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public JacoChatMessage JacoChatMessage { get { return JacoChatMessage.Parse(Message); } }
        public Int32 Bytes { get { return Message.Length; } }
    }

    public class PrivmsgRecievedEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }

    public class JoinRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }

    public class PartRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }

    public class QuitRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }

    public class NickRecievedEventArgs : EventArgs
    {
        public string OldNick { get; set; }
        public string NewNick { get; set; }
        public string Channel { get; set; }
    }

    public class NamesRecievedEventArgs : EventArgs
    {
        public string Channel { get; set; }
        public string List { get; set; }
    }

    public class TopicRecievedEventArgs : EventArgs
    {
        public string Channel { get; set; }
        public string Topic { get; set; }
    }

    public class WhoisRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Whois { get; set; }
    }

    public class KickRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }

    public class BanRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }

    public class ChanOpRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string GivenTaken { get; set; }
    }

    public class ErrorRecievedEventArgs : EventArgs
    {
        public string Error { get; set; }
    }

    public class UnknownRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
