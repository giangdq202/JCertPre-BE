namespace JCertPreApplication.Domain.Configuration;

/// <summary>
/// Configuration settings for LiveKit integration.
/// </summary>
public class LiveKitConfiguration
{
    public const string SectionName = "LiveKit";

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

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException("LiveKit ApiKey is required. Please configure LiveKit__ApiKey in your .env file.");

        if (string.IsNullOrWhiteSpace(ApiSecret))
            throw new InvalidOperationException("LiveKit ApiSecret is required. Please configure LiveKit__ApiSecret in your .env file.");

        if (string.IsNullOrWhiteSpace(ServerUrl))
            throw new InvalidOperationException("LiveKit ServerUrl is required. Please configure LiveKit__ServerUrl in your .env file.");

        if (!Uri.TryCreate(ServerUrl, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("LiveKit ServerUrl must be a valid URL.");
    }
} 