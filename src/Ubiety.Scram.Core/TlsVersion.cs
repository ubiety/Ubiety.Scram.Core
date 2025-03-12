namespace Ubiety.Scram.Core;

/// <summary>
/// Which version of TLS binding the user implementing.
/// </summary>
public enum TlsVersion
{
    /// <summary>
    /// TLS Server Endpoint
    /// </summary>
    TlsServerEndpoint = 0,

    /// <summary>
    /// TLS Unique
    /// </summary>
    TlsUnique = 1,

    /// <summary>
    /// TLS Exporter
    /// </summary>
    TlsExporter = 2,
}
