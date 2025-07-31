namespace JCertPreApplication.Domain.Enums
{
    /// <summary>
    /// Defines the different types of file buckets in Appwrite
    /// </summary>
    public enum AppwriteBucketType
    {
        /// <summary>
        /// Bucket for image files (JPEG, PNG, GIF, etc.)
        /// </summary>
        Images,

        /// <summary>
        /// Bucket for video files (MP4, AVI, MOV, etc.)
        /// </summary>
        Videos,

        /// <summary>
        /// Bucket for document files (PDF, DOC, TXT, etc.)
        /// </summary>
        Documents
    }

    /// <summary>
    /// File types supported by the application
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Image file type
        /// </summary>
        Image,

        /// <summary>
        /// Video file type
        /// </summary>
        Video,

        /// <summary>
        /// Document file type (raw files)
        /// </summary>
        Document
    }
}
