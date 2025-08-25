## Third-Party Services Used

This document lists external services and SDKs integrated in the project, their purposes, key configuration, and related code locations.

### Redis
- **Purpose**: Caching for performance and ephemeral data.
- **Key config (env)**: `Redis__ConnectionString`
- **Config class**: `JCertPreApplication.Domain/Configuration/RedisConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Cache/RedisClient.cs`
 - **Official Docs**:
   - StackExchange.Redis Documentation: [https://stackexchange.github.io/StackExchange.Redis/](https://stackexchange.github.io/StackExchange.Redis/)
   - Connecting to Redis on StackExchange.Redis (C#): [https://www.alibabacloud.com/help/en/apsaradb-for-redis/latest/connecting-to-redis-on-stackexchange-redis-c](https://www.alibabacloud.com/help/en/apsaradb-for-redis/latest/connecting-to-redis-on-stackexchange-redis-c)

### Appwrite (File Storage)
- **Purpose**: Object storage for images, videos, and documents.
- **Key config (env)**: `Appwrite__Endpoint`, `Appwrite__ProjectId`, `Appwrite__ApiKey`, `Appwrite__ImagesBucketId`, `Appwrite__VideosBucketId`, `Appwrite__DocumentsBucketId`, `Appwrite__Secure`, `Appwrite__MaxFileSizeMB`, `Appwrite__UploadTimeoutSeconds`
- **Config class**: `JCertPreApplication.Domain/Configuration/AppwriteConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/File/AppwriteFileService.cs`
- **Related contracts**: `JCertPreApplication.Application/Contracts/IFileService.cs`
 - **Official Docs**:
   - Appwrite Storage API Reference: [https://appwrite.io/docs/references/cloud/api/storage](https://appwrite.io/docs/references/cloud/api/storage)
   - Appwrite .NET SDK (GitHub): [https://github.com/appwrite/sdk-for-dotnet](https://github.com/appwrite/sdk-for-dotnet)
   - Upload and download files in Appwrite: [https://appwrite.io/docs/products/storage/upload-download](https://appwrite.io/docs/products/storage/upload-download)

### Firebase (Auth)
- **Purpose**: Authentication via Firebase Admin SDK (token verification).
- **Key config (env)**: `Firebase__Type`, `Firebase__ProjectId`, `Firebase__PrivateKeyId`, `Firebase__PrivateKey`, `Firebase__ClientEmail`, `Firebase__ClientId`, `Firebase__AuthUri`, `Firebase__TokenUri`, `Firebase__AuthProviderX509CertUrl`, `Firebase__ClientX509CertUrl`, `Firebase__UniverseDomain`
- **Config class**: `JCertPreApplication.Domain/Configuration/FirebaseConfiguration.cs`
- **Usage**: `JCertPreApplication.Application/Features/Auth/AuthService.cs`, API controller `JCertPreApplication.API/Controllers/AuthController.cs`
 - **Official Docs**:
   - Add the Firebase Admin SDK to your server (.NET): [https://firebase.google.com/docs/admin/setup?hl=en#add_the_sdk](https://firebase.google.com/docs/admin/setup?hl=en#add_the_sdk)
   - Verify ID Tokens with Firebase Admin SDK (.NET): [https://firebase.google.com/docs/auth/admin/verify-id-tokens](https://firebase.google.com/docs/auth/admin/verify-id-tokens)
   - Firebase Admin SDK for .NET Announcement: [https://firebase.googleblog.com/2018/08/introducing-firebase-admin-sdk-for-net.html](https://firebase.googleblog.com/2018/08/introducing-firebase-admin-sdk-for-net.html)

### LiveKit
- **Purpose**: Real-time audio/video for livestream sessions.
- **Key config (env)**: `LiveKit__ApiKey`, `LiveKit__ApiSecret`, `LiveKit__ServerUrl`
- **Config class**: `JCertPreApplication.Domain/Configuration/LiveKitConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/LiveKit/LiveKitService.cs`
- **Contracts/Features**: `JCertPreApplication.Application/Contracts/ILiveKitService.cs`, `JCertPreApplication.Application/Features/Livestreams/LivestreamService.cs`
- **API**: `JCertPreApplication.API/Controllers/LiveKitController.cs`
 - **Official Docs**:
   - LiveKit Server API Documentation: [https://docs.livekit.io/references/server-apis/](https://docs.livekit.io/references/server-apis/)
   - Generating Tokens (for backend): [https://docs.livekit.io/guides/token-generation/](https://docs.livekit.io/guides/token-generation/)
   - LiveKit .NET Server SDK: [https://www.nuget.org/packages/Livekit.Server.Sdk.Dotnet](https://www.nuget.org/packages/Livekit.Server.Sdk.Dotnet)
   - LiveKit .NET Server Tutorial: [https://docs.livekit.io/tutorials/server/dotnet/](https://docs.livekit.io/tutorials/server/dotnet/)

### PayOS
- **Purpose**: Payment processing (create orders, verify callbacks).
- **Key config (env)**: `PayOS__ClientId`, `PayOS__ApiKey`, `PayOS__ChecksumKey`, `PayOS__ReturnUrl`, `PayOS__CancelUrl`
- **Config class**: `JCertPreApplication.Domain/Configuration/PayOSConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/PayOSService.cs`
- **Features**: `JCertPreApplication.Application/Features/Payment/PaymentService.cs`
 - **Official Docs**:
   - PayOS Developer Documentation (General): [https://payos.vn/docs/](https://payos.vn/docs/)
   - Note: Specific API endpoints for creating orders and handling callbacks are within the PayOS docs.

### Brevo SMTP (Email)
- **Purpose**: Transactional email (welcome, reset password, notifications) via Brevo SMTP.
- **Key config (env)**: `Smtp__Host`, `Smtp__Port`, `Smtp__EnableSsl`, `Smtp__Username`, `Smtp__Password`, `Smtp__FromEmail`, `Smtp__FromName`, `Smtp__Timeout`
- **Notes**: Use Brevo SMTP settings (e.g., host `smtp-relay.brevo.com`, port `587`, username is API key).
- **Config class**: `JCertPreApplication.Domain/Configuration/SmtpConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/Mail/MailService.cs`
- **Templates**: `JCertPreApplication.Persistence/Templates/*`
 - **Official Docs**:
   - Send transactional emails using Brevo SMTP: [https://www.brevo.com/docs/send-transactional-emails/smtp/](https://www.brevo.com/docs/send-transactional-emails/smtp/)
   - Brevo API Reference (Transactional Emails - Send a transactional email): [https://developers.brevo.com/docs/send-a-transactional-email](https://developers.brevo.com/docs/send-a-transactional-email)
   - How to send emails in ASP.NET Web Application Using Sendinblue / Brevo API: [https://www.youtube.com/watch?v=0kI-8N7z_iE](https://www.youtube.com/watch?v=0kI-8N7z_iE)

### Google Gemini (Generative AI)
- **Purpose**: AI assistance (content generation or evaluation within features).
- **Key config (env)**: `GeminiAI__ApiKey`, `GeminiAI__BaseUrl`, `GeminiAI__Model`, `GeminiAI__RateLimitRPM`, `GeminiAI__TimeoutSeconds`, `GeminiAI__MaxRetries`
- **Config class**: `JCertPreApplication.Domain/Configuration/GeminiConfiguration.cs`
- **Models**: `JCertPreApplication.Persistence/Models/GeminiModels.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/GeminiService.cs`
- **Docs**: `Documents/AI_INTEGRATION.md`
 - **Official Docs**:
   - Google Gemini API Documentation: [https://ai.google.dev/gemini-api/docs](https://ai.google.dev/gemini-api/docs)
   - Quickstart: Use Gemini with .NET: [https://jokim.blog/2024/04/02/quickstart-use-gemini-with-net-google-ai/](https://jokim.blog/2024/04/02/quickstart-use-gemini-with-net-google-ai/)
   - Integrating with Google Gemini (.NET SDK): [https://dev.to/gsilvamartin/integrating-with-google-gemini-using-a-built-net-sdk-1n5f](https://dev.to/gsilvamartin/integrating-with-google-gemini-using-a-built-net-sdk-1n5f)

- **Purpose**: Primary relational database for the application.
- **Key config (env)**: `ConnectionStrings__JCertPreDB`
- **DbContext**: `JCertPreApplication.Persistence/DatabaseContext/JCertPreDatabaseContext.cs`
- **Migrations**: `JCertPreApplication.Persistence/Migrations/*`
### PostgreSQL (Database)
- **Purpose**: Primary relational database for the application.
- **Key config (env)**: `ConnectionStrings__JCertPreDB`
- **DbContext**: `JCertPreApplication.Persistence/DatabaseContext/JCertPreDatabaseContext.cs`
- **Migrations**: `JCertPreApplication.Persistence/Migrations/*`
 - **Official Docs**:
   - Npgsql Documentation (.NET Data Provider for PostgreSQL): [https://www.npgsql.org/doc/](https://www.npgsql.org/doc/)
   - Azure PostgreSQL SDK for .NET overview: [https://learn.microsoft.com/en-us/dotnet/azure/postgresql/overview](https://learn.microsoft.com/en-us/dotnet/azure/postgresql/overview)

### Official Docs
- LiveKit: [https://docs.livekit.io/home/](https://docs.livekit.io/home/)
- Appwrite: [https://appwrite.io/docs](https://appwrite.io/docs)
- PayOS: [https://payos.vn/docs/](https://payos.vn/docs/)
- Firebase Authentication: [https://firebase.google.com/docs/auth](https://firebase.google.com/docs/auth)
- Brevo (SMTP/API): [https://developers.brevo.com/docs/getting-started](https://developers.brevo.com/docs/getting-started)
- Google Gemini API: [https://ai.google.dev/gemini-api/docs](https://ai.google.dev/gemini-api/docs)
---

If you add a new external dependency, please update:
- This document with purpose, env keys, and references
- `env.example` with the new configuration keys
- A corresponding configuration class under `JCertPreApplication.Domain/Configuration`

