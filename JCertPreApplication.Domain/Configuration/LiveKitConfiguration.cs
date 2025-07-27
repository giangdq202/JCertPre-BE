namespace JCertPreApplication.Domain.Configuration;

/// <summary>
/// Configuration settings for LiveKit integration.
/// </summary>
public class LiveKitConfiguration
{
    /// <summary>
    /// The API Key used to authenticate with LiveKit server.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The API Secret used to sign tokens for LiveKit server.
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>
    /// The LiveKit server URL.
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;
} 