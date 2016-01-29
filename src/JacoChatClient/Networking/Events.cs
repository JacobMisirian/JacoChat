using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JacoChatClient
{
    /// <summary>
    /// Message recieved event arguments.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public JacoChatMessage JacoChatMessage { get { return JacoChatMessage.Parse(Message); } }
        public Int32 Bytes { get { return Message.Length; } }
    }
    /// <summary>
    /// Privmsg recieved event arguments.
    /// </summary>
    public class PrivmsgRecievedEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }
    /// <summary>
    /// Join recieved event arguments.
    /// </summary>
    public class JoinRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }
    /// <summary>
    /// Part recieved event arguments.
    /// </summary>
    public class PartRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }
    /// <summary>
    /// Quit recieved event arguments.
    /// </summary>
    public class QuitRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }
    /// <summary>
    /// Nick recieved event arguments.
    /// </summary>
    public class NickRecievedEventArgs : EventArgs
    {
        public string OldNick { get; set; }
        public string NewNick { get; set; }
        public string Channel { get; set; }
    }
    /// <summary>
    /// Names recieved event arguments.
    /// </summary>
    public class NamesRecievedEventArgs : EventArgs
    {
        public string Channel { get; set; }
        public string List { get; set; }
    }
    /// <summary>
    /// Topic recieved event arguments.
    /// </summary>
    public class TopicRecievedEventArgs : EventArgs
    {
        public string Channel { get; set; }
        public string Topic { get; set; }
    }
    /// <summary>
    /// Whois recieved event arguments.
    /// </summary>
    public class WhoisRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Whois { get; set; }
    }
    /// <summary>
    /// Kick recieved event arguments.
    /// </summary>
    public class KickRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string Reason { get; set; }
    }
    /// <summary>
    /// Ban recieved event arguments.
    /// </summary>
    public class BanRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }
    /// <summary>
    /// Un ban recieved event arguments.
    /// </summary>
    public class UnBanRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
    }
    /// <summary>
    /// Chan op recieved event arguments.
    /// </summary>
    public class ChanOpRecievedEventArgs : EventArgs
    {
        public string User { get; set; }
        public string Channel { get; set; }
        public string GivenTaken { get; set; }
    }
    /// <summary>
    /// List recieved event arguments.
    /// </summary>
    public class ListRecievedEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
    /// <summary>
    /// Error recieved event arguments.
    /// </summary>
    public class ErrorRecievedEventArgs : EventArgs
    {
        public string Error { get; set; }
    }
    /// <summary>
    /// Unknown recieved event arguments.
    /// </summary>
    public class UnknownRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    /// <summary>
    /// Motd recieved event arguments.
    /// </summary>
    public class MotdRecievedEventArgs : EventArgs
    {
        public string Line { get; set; }
    }
}
