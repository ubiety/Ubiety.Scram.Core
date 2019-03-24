---
title: Getting Started
---

# Getting Started

The best way to install Ubiety SCRAM is to use your favorite NuGet package manager.

# [Package Manager](#tab/pm)
```ps
Install-Package Ubiety.Scram.Core
```
# [.NET CLI](#tab/cli)
```bash
dotnet add package Ubiety.Scram.Core
```
***

Once it is added to your project you can start the authentication process by creating a new `ClientFirstMessage`.
Ubiety SCRAM is platform agnostic so you can use any transportation mechanism you wish to complete the process.

```cs
var firstMessage = new ClientFirstMessage("username", "random-nonce");
```

The nonce should be something unique to the client and the current authentication session. Once you have the new message
object you can send it.

```cs
var client = new TcpClient();

client.Send(firstMessage.Message);
```
