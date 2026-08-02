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

The token and the status have to agree, and `ClientFinalMessage` throws `ArgumentException` when
they do not:

- `Required` without a token would advertise a binding the message does not carry.
- A token under `NotSupported` or `ClientSupport` would append binding data that RFC 5802 only
  permits behind a `p=` header.

Both produce a message a conforming server rejects, so the exception replaces an authentication
failure that otherwise gives no hint as to its cause.

## Getting the token out of .NET

This is the awkward part, and it constrains which binding type is actually reachable.

`tls-server-end-point` is computable today: it is the hash of the server certificate, which you can
take from `SslStream.RemoteCertificate`.

`tls-exporter` is not. RFC 9266 defines it as the TLS exporter output for label
`EXPORTER-Channel-Binding` with an empty context and a length of 32 bytes, and as of .NET 10
`SslStream` exposes no keying material export API to produce it. Both the API and the feature are
still open proposals upstream:

- [dotnet/runtime#112529](https://github.com/dotnet/runtime/issues/112529) — Export Keying Material
  for TLS sessions
- [dotnet/runtime#73118](https://github.com/dotnet/runtime/issues/73118) — RFC 9266 channel
  bindings

Beware of sample code calling `SslStream.ExportKeyingMaterial`: it matches the shape of the
proposed API, but no shipped .NET version has that method. Until it lands, using
`TlsVersion.TlsExporter` means obtaining the exporter value from a TLS implementation other than
`SslStream` — native interop or a managed stack that exposes one. The library encodes whatever you
hand it correctly; it just cannot obtain it for you. When the API ships, this code path works
unchanged — callers simply gain a way to fill the argument.

`tls-unique` is likewise unavailable from `SslStream`, and RFC 9266 notes it is not defined for
TLS 1.3 at all.

Pass whichever token you obtained, and make sure `TlsVersion` names the same binding type — the
name goes into the signed header, so a mismatch fails the exchange.
