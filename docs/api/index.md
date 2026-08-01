# API reference

Generated from the source. Every public type and member is documented in the code itself, so this
reference cannot drift from what the library actually does.

## Where to start

- <xref:Ubiety.Scram.Core.Messages.ClientFirstMessage> — opens the exchange
- <xref:Ubiety.Scram.Core.Messages.ServerFirstMessage> — the salt, nonce and iteration count
- <xref:Ubiety.Scram.Core.Messages.ClientFinalMessage> — computes the proof
- <xref:Ubiety.Scram.Core.Messages.ServerFinalMessage> — the signature to verify
- <xref:Ubiety.Scram.Core.Hash> — SHA-1, SHA-256 and SHA-512 for the mechanism in use

## Namespaces

- <xref:Ubiety.Scram.Core> — hashing, SASLprep and the channel binding enums
- <xref:Ubiety.Scram.Core.Messages> — the four messages of the exchange
- <xref:Ubiety.Scram.Core.Attributes> — the attributes those messages carry
- <xref:Ubiety.Scram.Core.Exceptions> — what parsing throws
