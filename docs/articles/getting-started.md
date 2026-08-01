# Getting started

## Install

```shell
dotnet package add Ubiety.Scram.Core
```

## The exchange

SCRAM is four messages: the client goes first, the server replies with a salt and an iteration
count, the client proves it knows the password, and the server proves it too. This library builds
and parses all four; sending them is your job.

### 1. Client first

```csharp
using Ubiety.Scram.Core;
using Ubiety.Scram.Core.Messages;

var clientFirst = new ClientFirstMessage("user", "fyko+d2lbbFgONRv9qkxdawL");

// n,,n=user,r=fyko+d2lbbFgONRv9qkxdawL
Send(clientFirst.Message);
```

The nonce is yours to generate and must be fresh for every exchange. Anything that produces
unpredictable printable characters will do:

```csharp
var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
```

The username is prepared with SASLprep before it goes on the wire, so pass it as the user typed it.

### 2. Server first

```csharp
var serverFirst = ServerFirstMessage.Parse(reply);

serverFirst.Iterations.Value;  // 4096
serverFirst.Salt.Value;        // byte[]
serverFirst.Nonce.Value;       // your nonce with the server's appended
```

Use `TryParse` if a malformed reply should not throw:

```csharp
if (!ServerFirstMessage.TryParse(reply, out var serverFirst))
{
    throw new AuthenticationException("The server sent a message we could not read.");
}
```

### 3. Client final

Building the final message derives the salted password and computes the proof. The hash must match
the mechanism the server offered — `SCRAM-SHA-256` means <xref:Ubiety.Scram.Core.Hash.Sha256>.

```csharp
var clientFinal = new ClientFinalMessage(clientFirst, serverFirst, "pencil", Hash.Sha256());

// c=biws,r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,p=v0X8v3Bz2T0CJGbJQyF0X+HI4Ts=
Send(clientFinal.Message);
```

Deriving the key runs the iteration count the server asked for, so this call is deliberately slow.

### 4. Server final, and verifying it

The exchange is not finished when the server accepts the proof. The server also has to prove it
holds the stored key, and a client that skips this check will authenticate happily against an
impostor.

```csharp
var serverFinal = ServerFinalMessage.Parse(reply);

if (serverFinal.ServerSignature != clientFinal.ServerSignature)
{
    throw new AuthenticationException("The server failed to prove it knows the password.");
}
```

The `==` operator on <xref:Ubiety.Scram.Core.Attributes.ServerSignatureAttribute> compares in
constant time, so use it rather than pulling out the strings and comparing them yourself.

## Choosing a hash

| Mechanism | Factory |
| --- | --- |
| `SCRAM-SHA-1` | <xref:Ubiety.Scram.Core.Hash.Sha1> |
| `SCRAM-SHA-256` | <xref:Ubiety.Scram.Core.Hash.Sha256> |
| `SCRAM-SHA-512` | <xref:Ubiety.Scram.Core.Hash.Sha512> |

`SCRAM-SHA-1` exists because RFC 5802 defines it and servers still offer it. Prefer SHA-256 or
SHA-512 when the server supports them.

## Bytes instead of strings

Every message converts implicitly to and from `byte[]` and `string`, which saves a round of
encoding when the transport hands you raw bytes:

```csharp
byte[] payload = clientFirst;
ServerFirstMessage serverFirst = responseBytes;
```
