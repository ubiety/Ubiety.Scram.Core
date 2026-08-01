# Channel binding

Channel binding ties a SCRAM exchange to the TLS connection underneath it, so an attacker who
terminates TLS in the middle cannot forward the authentication to the real server. The client
declares its intent in the GS2 header of the first message, and repeats that declaration inside the
signed `c=` attribute of the final message — if anything rewrote the header in flight, the proof no
longer verifies.

## Declaring intent

<xref:Ubiety.Scram.Core.Messages.ClientFirstMessage> takes the binding status and, when binding is
in use, which TLS binding type the connection provides:

```csharp
var clientFirst = new ClientFirstMessage(
    "user",
    nonce,
    ChannelBindingStatus.Required,
    TlsVersion.TlsExporter);
```

<xref:Ubiety.Scram.Core.ChannelBindingStatus> has three values, and they produce three different
GS2 headers:

| Status | Header | Meaning |
| --- | --- | --- |
| `NotSupported` | `n,,` | This client cannot do channel binding. |
| `ClientSupport` | `y,,` | This client can, but the server did not advertise a `-PLUS` mechanism. |
| `Required` | `p=<type>,,` | Binding is in use, over the named binding type. |

`ClientSupport` is the one that earns its keep. Sending `y` when the server offered no `-PLUS`
mechanism records that the client would have bound if asked; a downgrade attacker who strips the
`-PLUS` mechanism from the server's list cannot also change the `y` without breaking the proof, so
the server detects the strip.

`NotSupported` is the default, and produces the plain `n,,` header used by every non-binding
exchange.

## Binding types

<xref:Ubiety.Scram.Core.TlsVersion> names the binding type that goes into the `p=` header:

| Value | Header | Defined by |
| --- | --- | --- |
| `TlsExporter` | `p=tls-exporter` | [RFC 9266](https://datatracker.ietf.org/doc/html/rfc9266) |
| `TlsUnique` | `p=tls-unique` | [RFC 5929](https://datatracker.ietf.org/doc/html/rfc5929) |
| `TlsServerEndpoint` | `p=tls-server-end-point` | [RFC 5929](https://datatracker.ietf.org/doc/html/rfc5929) |

`tls-unique` is the default for backwards compatibility, but it is not defined for TLS 1.3 — use
`tls-exporter` on a TLS 1.3 connection.

## Supplying the token

The binding data itself comes from the TLS stack, not from this library, and is passed to
<xref:Ubiety.Scram.Core.Messages.ClientFinalMessage> as the optional `token` argument:

```csharp
var clientFinal = new ClientFinalMessage(
    clientFirst,
    serverFirst,
    "pencil",
    Hash.Sha256(),
    channelBindingToken);
```

The `c=` attribute is then `base64(gs2-header + token)`, as RFC 5802 requires. Omit the token and
the attribute is `base64(gs2-header)` alone — which is what `n,,` exchanges send, and why an
unbound client final message always carries `c=biws`.

Getting the token out of .NET's TLS stack is the awkward part.
`SslStream.NegotiatedCipherSuite` and friends do not expose `tls-unique` or `tls-exporter`, so on
most platforms this means either `tls-server-end-point` — the hash of the server certificate,
which you can compute from `SslStream.RemoteCertificate` — or a native handle into the platform's
TLS library. Pass whichever you obtained, and make sure `TlsVersion` names the same one.
