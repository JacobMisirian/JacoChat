# JacoChat

JacoChat is a text based chat protocol in the format of a client
server application written in C# that mimicks that of IRC.
[JacoChat Library Documentation](http://misiriansoft.com/JacoChatDocs)

## JacoChat Protocol: Client to Server
This section covers the different commmands and format of messages
that a JacoChat client would send to a JacoChat server.

### NICK
The nick command must and should be the first thing that is sent
to the server upon a client connect. It gives the server a way
to identify the client and allow them to join a channel, etc.

The format of NICK is:
```
NICK [NICKNAME]
```

For example upon connect your client might send the server:
```
NICK ComputerCam
```

### JOIN
The join command is more than likely the next thing that a client
should send to the server. It allows you to join or create a
channel, where your client can talk to other clients. Like most
IRC-like protocols, channels must start with a # to be valid.

The format of JOIN is:
```
JOIN #[CHANNEL]
```

For example your client might send:
```
JOIN #int0x10
```

It is important to note than upon joining a channel the server
will send you your own JOIN message, the TOPIC of the channel,
and the NAMES list for that channel. More on these can be found
in the Server to Client section down below.

### PART
The part command is for leaving a channel that you no longer
wish to be in. It de-registers you from that channel and your
client will no longer recieve or be able to send to that
channel until you JOIN again.

When you issue a PART you provide the channel name and the reason
for your part in the following format:
```
PART #[CHANNEL] [REASON]
```

For example:
```
PART #int0x10 Programming is for nerds
```

### PRIVMSG
The privmsg command sends a message to a user or channel and 
is the primary method of communicating in JacoChat. You can
either send to a channel where everyone can see the message
or you can send it to a specific user in the form of a PM.

Format:
```
PRIVMSG [#[CHANNEL]/[USER]] [MESSAGE]
```

For example:
PRIVMSG #int0x10 Hello, World!

### NAMES
The names command requests the server to send you a list of
names of the users in the current channel. The server will
send these back in a NAMES message, which is talked about
in the Server to Client section down below.

Format:
```
NAMES #[CHANNEL]
```

For example:
```
NAMES #int0x10
```

### TOPIC
The topic command requests the server to send you the topic
of a channel that you are in. The server will send the topic
to your client in a TOPIC message, which is talked about
in the Server to Client section down below.

Format:
```
TOPIC #[CHANNEL]
```

For example:
```
TOPIC #ncagate
```

In addition if you are a channel OP you have the ability to
set the topic using the topic command with an extra argument
at the end of your request.

Format:
```
TOPIC #[CHANNEL] [TOPIC]
```

For example:
```
TOPIC #int0x10 This is a new topic
```

### WHOIS
The whois command is used to return information about a
desired user. This information includes the nickname, IP,
connection time, time idle, and PING of the user. The
server will return this information with several WHOIS
messages, which will be talked about in the Server to
Client section down below.

Format:
```
WHOIS [USER]
```

For example:
```
WHOIS GruntTheSkid
```

### KICK
The kick command is used by channel OPs to forcefully remove
a user from the channel if the user is getting out of hand.
This will prevent the user from sending or recieving any 
messages from the channel nor will that user be a part of
said channel anymore. Note that this not prevent the user
from rejoining the channel, and to prevent this you should
couple this with the BAN commmand.

Format:
```
KICK [USER] #[CHANNEL] [REASON]
```

For exmaple:
```
KICK timeScrub #int0x10 No sanicking!
```

### BAN
The ban command is used by channel OPs to prevent a user
from sending messages to the channel or otherwise interacting
with it. It is important to note that this does not kick
said user from the channel, which can be accomplished using
the KICK command (above).

Format:
```
BAN [USER] #[CHANNEL]
```

For example:
```
BAN haxx0rPr0ny #ncagate
```

### UNBAN
The unban command is used by channel OPs to remove a ban
placed on a user.

Format:
```
UNBAN [USER] #[CHANNEL]
```

For example:
```
UNBAN csharper #nca
```

### CHANOP
The chanop command is used by channel OPs and network OPs
to give or take channel OP for a user.

Format:
```
CHANOP #[CHANNEL] [USER] [GIVE/TAKE]
```

For example:
```
CHANOP #int0x10 Reagan give
```

### LIST
The list command is used to return from the server a list
of all the channels on the server, the amount of users, OPs,
and the topic of each channel. The server will return these
using a list message talked about in the Server to Client 
section below.

Format and example:
```
LIST
```

## JacoChat Protocol: Server to Client
This section covers the types and format of messages that
a JacoChat server would send to a client.

### PRIVMSG
The privmsg command denotates that someone has sent your client
a message either from sending it to a channel you are in or
by sending it to you directly as a PM.

Format:
```
[SENDER] PRIVMSG [#[CHANNEL]/[USER]] :[MESSAGE]
```

For example:
```
reagan PRIVMSG #int0x10 :Hello, World!
```

### JOIN
The join command is sent whenever a user joins a channel that
you are in (including when you join that channel).

Format:
```
[USER] JOIN #[CHANNEL]
```

For example:
```
OverlordSatan JOIN #pptosn
```

### PART
The part command is sent whenever a user leaves a channel that
you are in.

Format:
```
[USER] PART #[CHANNEL] :[REASON]
```

For example:
```
keeperOfTime PART #int0x10 :I quit
```

### NICK
The nick command is sent whenever a user (including yourself)
changes their nickname in a channel you are in.

Format:
```
[OLD_NICK] NICK #[CHANNEL] [NEW_NICK]
```

For example:
```
Reagan NICK #int0x10 TheReaganKeeper
```

### QUIT
The quit command is sent whenever a user that was in a channel
that you are in kills their connection to the server.

Format:
```
[USER] QUIT #[CHANNEL] :[REASON]
```

For example:
```
tiezerk QUIT #int0x10 :Ping timeout: 10 seconds
```

### TOPIC
The topic command is sent when your client joins a channel, whenever
that client specifically requests the topic using the TOPIC
Client to Server command, or when a channel OP changes the
topic of the channel.

Format:
```
server TOPIC #[CHANNEL] :[TOPIC]
```

For example:
```
server TOPIC #auroragate :Where is he?
```

### KICK
The kick command is sent whenever a channel OP kicks a user
from a channel you are in.

Format:
```
[USER] KICK #[CHANNEL] :[REASON]
```

For example:
```
Aviv KICK #vbosn :Do not.
```

### BAN
The ban command is sent whenever a channel OP bans a user
from a channel you are in.

Format:
```
[USER] BAN #[CHANNEL]
```

For example:
```
Fabtop BAN #hassium
```

### UNBAN
The unban command is sent whenever a channel OP unbans
a user from a channel you are in.

Format:
```
[USER] UNBAN #[CHANNEL]
```

For example:
```
Putin UNBAN #crimea
```

### WHOIS
The whois command is sent whenever a client requests
user information using a Client to Server WHOIS command.
It is important to note that the server sends out several
whois commands are sent out foreach request, one foreach
catagory of a whois.

Format:
```
server WHOIS [USER] :[DATA]
```

For example:
```
server WHOIS nalz :From: 127.0.0.1
```

### CHANOP
The chanop command is sent whenever a client that is
in the same channel as you (or you) has had their channel
operator given or taken.

Format:
```
[OP_NICK] CHANOP #[CHANNEL] [USER] :[GIVE/TAKE]
```

For example:
```
Viper CHANOP #int0x10 Tiezerk :GIVE
```


### ERROR
The error command is sent whenever you have tried to send
something invalid or do something invalid.

Format:
```
server ERROR :[MESSAGE]
```

For example:
```
server ERROR :No such channel #FrenchMilitaryVictories
```
