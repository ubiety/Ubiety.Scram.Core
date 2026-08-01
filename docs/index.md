---
_layout: landing
---

# Ubiety.Scram.Core

An implementation of SCRAM — the Salted Challenge Response Authentication Mechanism of
[RFC 5802](https://datatracker.ietf.org/doc/html/rfc5802) — for .NET. It builds the client
messages, parses the server's replies, and computes the proof and signature.

```csharp
var clientFirst = new ClientFirstMessage("user", nonce);
// send clientFirst.Message, read the reply

var serverFirst = ServerFirstMessage.Parse(reply);

var clientFinal = new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256());
// send clientFinal.Message, read the reply

var serverFinal = ServerFinalMessage.Parse(finalReply);

if (serverFinal.ServerSignature != clientFinal.ServerSignature)
{
    throw new AuthenticationException("The server failed to prove it knows the password.");
}
```

- [Getting started](articles/getting-started.md) — install, run an exchange, verify the server
- [Channel binding](articles/channel-binding.md) — GS2 headers, binding status, TLS versions
- [Message reference](articles/message-reference.md) — the four messages and their attributes
- [API reference](api/index.md) — generated from the source

The library does not open sockets or speak any particular protocol. It produces and consumes the
strings SCRAM defines, and leaves the transport — SASL over XMPP, LDAP, PostgreSQL or anything
else — to the caller.
