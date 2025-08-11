using System.Text.RegularExpressions;

namespace JCertPreApplication.Application.Utilities
{
    /// <summary>
    /// Utility class for parsing file URLs and extracting public IDs
    /// </summary>
    public static class FileUrlParser
    {
        // Appwrite URL pattern: https://appwrite.zd-dev.xyz/v1/storage/buckets/{bucketId}/files/{fileId}/view?project={projectId}
        private static readonly Regex AppwriteUrlPattern = new Regex(
            @"/storage/buckets/[^/]+/files/([^/]+)/",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Alternative patterns for different storage providers (future extensibility)
        private static readonly Dictionary<string, Regex> ProviderPatterns = new()
        {
            ["appwrite"] = AppwriteUrlPattern,
            // Can add more providers like:
            // ["cloudinary"] = new Regex(@"/image/upload/(?:v\d+/)?([^/]+)\."),
            // ["aws"] = new Regex(@"/([^/]+)$"),
        };

        /// <summary>
        /// Extracts the public ID from an Appwrite file URL
        /// </summary>
        /// <param name="fileUrl">The complete file URL</param>
        /// <returns>The extracted file ID, or null if extraction fails</returns>
        public static string? ExtractAppwriteFileId(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return null;

            try
            {
                var match = AppwriteUrlPattern.Match(fileUrl);
                return match.Success ? match.Groups[1].Value : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts public ID from a file URL, attempting to detect the provider automatically
        /// </summary>
        /// <param name="fileUrl">The complete file URL</param>
        /// <returns>The extracted public ID, or null if extraction fails</returns>
        public static string? ExtractPublicId(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return null;

            // Try Appwrite pattern first (primary provider)
            var appwriteId = ExtractAppwriteFileId(fileUrl);
            if (!string.IsNullOrEmpty(appwriteId))
                return appwriteId;

            // Could add logic for other providers here
            // For now, fallback to Appwrite extraction
            return null;
        }

        /// <summary>
        /// Validates if a URL matches the expected Appwrite pattern
        /// </summary>
        /// <param name="fileUrl">The URL to validate</param>
        /// <returns>True if the URL matches Appwrite pattern, false otherwise</returns>
        public static bool IsValidAppwriteUrl(string? fileUrl)
        {
            return !string.IsNullOrWhiteSpace(fileUrl) && AppwriteUrlPattern.IsMatch(fileUrl);
        }

        /// <summary>
        /// Extracts bucket ID from an Appwrite file URL
        /// </summary>
        /// <param name="fileUrl">The complete file URL</param>
        /// <returns>The extracted bucket ID, or null if extraction fails</returns>
        public static string? ExtractAppwriteBucketId(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return null;

            try
            {
                var bucketPattern = new Regex(@"/storage/buckets/([^/]+)/files/", RegexOptions.IgnoreCase);
                var match = bucketPattern.Match(fileUrl);
                return match.Success ? match.Groups[1].Value : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates if the file URL belongs to the expected project
        /// </summary>
        /// <param name="fileUrl">The complete file URL</param>
        /// <param name="expectedProjectId">The expected project ID</param>
        /// <returns>True if URL belongs to the expected project, false otherwise</returns>
        public static bool IsFromExpectedProject(string? fileUrl, string? expectedProjectId)
        {
            if (string.IsNullOrWhiteSpace(fileUrl) || string.IsNullOrWhiteSpace(expectedProjectId))
                return false;

            try
            {
                var projectPattern = new Regex(@"[?&]project=([^&]+)", RegexOptions.IgnoreCase);
                var match = projectPattern.Match(fileUrl);
                return match.Success && match.Groups[1].Value.Equals(expectedProjectId, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
