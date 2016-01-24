# JacoChat

JacoChat is a text based chat protocol in the format of a client
server application written in C# that mimicks that of IRC.

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
KICK [USER] [CHANNEL] [REASON]
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
BAN [USER] [CHANNEL]
```

For example:
```
BAN haxx0rPr0ny #ncagate
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
