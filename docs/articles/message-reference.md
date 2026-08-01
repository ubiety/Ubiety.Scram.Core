# Message reference

Every SCRAM message is a comma-separated list of `<name>=<value>` attributes. This library models
each attribute as a <xref:Ubiety.Scram.Core.Attributes.ScramAttribute> subclass, and each message
as a class that owns the attributes it is allowed to carry.

## The four messages

| Class | On the wire | Direction |
| --- | --- | --- |
| <xref:Ubiety.Scram.Core.Messages.ClientFirstMessage> | `n,,n=user,r=<nonce>` | client → server |
| <xref:Ubiety.Scram.Core.Messages.ServerFirstMessage> | `r=<nonce>,s=<salt>,i=<iterations>` | server → client |
| <xref:Ubiety.Scram.Core.Messages.ClientFinalMessage> | `c=<binding>,r=<nonce>,p=<proof>` | client → server |
| <xref:Ubiety.Scram.Core.Messages.ServerFinalMessage> | `v=<signature>` | server → client |

Each one exposes a `Message` property holding the full text, and the messages the client receives
have static `Parse` and `TryParse` methods. `Parse` throws
<xref:Ubiety.Scram.Core.Exceptions.MessageParseException> on a message it cannot read; `TryParse`
returns `false` instead.

Two messages also expose the partial forms the signature is computed over —
`ClientFirstMessage.BareMessage` and `ClientFinalMessage.MessageWithoutProof`. They are public
because implementing a SCRAM *server* needs them; a client does not have to touch either.

## Attributes

| Char | Class | Carries |
| --- | --- | --- |
| `n` | <xref:Ubiety.Scram.Core.Attributes.UserAttribute> | Username, SASLprep-normalised, with `=` and `,` escaped |
| `r` | <xref:Ubiety.Scram.Core.Attributes.NonceAttribute> | Client nonce, then client + server nonce |
| `s` | <xref:Ubiety.Scram.Core.Attributes.SaltAttribute> | Salt, base64 on the wire, `byte[]` in memory |
| `i` | <xref:Ubiety.Scram.Core.Attributes.IterationsAttribute> | PBKDF2 iteration count |
| `c` | <xref:Ubiety.Scram.Core.Attributes.ChannelAttribute> | `base64(gs2-header + binding token)` |
| `p` | <xref:Ubiety.Scram.Core.Attributes.ClientProofAttribute> | The client's proof |
| `v` | <xref:Ubiety.Scram.Core.Attributes.ServerSignatureAttribute> | The server's signature |
| `a` | <xref:Ubiety.Scram.Core.Attributes.AuthorizationIdentityAttribute> | Authorization identity, when it differs from the username |

The GS2 header that leads a client-first message is
<xref:Ubiety.Scram.Core.Attributes.Gs2Attribute>; see [channel binding](channel-binding.md).

Anything the library does not recognise becomes an
<xref:Ubiety.Scram.Core.Attributes.UnknownAttribute> rather than a parse failure, which is what
RFC 5802 asks for — extension attributes must be tolerated, not rejected.

## Comparing signatures

<xref:Ubiety.Scram.Core.Attributes.ServerSignatureAttribute> defines `==` and `!=` over a
constant-time comparison. Verifying the server's proof through those operators keeps the check off
the timing side channel:

```csharp
if (serverFinal.ServerSignature != clientFinal.ServerSignature)
{
    throw new AuthenticationException("The server failed to prove it knows the password.");
}
```

## Hashing

<xref:Ubiety.Scram.Core.Hash> wraps the three hash functions SCRAM is defined over and exposes the
three operations the mechanism needs:

- `ComputeHash(byte[])` — a plain digest, used for `StoredKey := H(ClientKey)`
- `ComputeHash(byte[], byte[])` — HMAC, used for the client and server signatures
- `ComputeHash(byte[], IEnumerable<byte>, int)` — PBKDF2, used for `SaltedPassword`

The message classes call these for you. They are public because a SCRAM server implementation
needs the same primitives to verify what a client sends.
