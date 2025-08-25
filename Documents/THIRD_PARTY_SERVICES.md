## Third-Party Services Used

This document lists external services and SDKs integrated in the project, their purposes, key configuration, and related code locations.

### Redis
- **Purpose**: Caching for performance and ephemeral data.
- **Key config (env)**: `Redis__ConnectionString`
- **Config class**: `JCertPreApplication.Domain/Configuration/RedisConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Cache/RedisClient.cs`

### Appwrite (File Storage)
- **Purpose**: Object storage for images, videos, and documents.
- **Key config (env)**: `Appwrite__Endpoint`, `Appwrite__ProjectId`, `Appwrite__ApiKey`, `Appwrite__ImagesBucketId`, `Appwrite__VideosBucketId`, `Appwrite__DocumentsBucketId`, `Appwrite__Secure`, `Appwrite__MaxFileSizeMB`, `Appwrite__UploadTimeoutSeconds`
- **Config class**: `JCertPreApplication.Domain/Configuration/AppwriteConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/File/AppwriteFileService.cs`
- **Related contracts**: `JCertPreApplication.Application/Contracts/IFileService.cs`

### Firebase (Auth)
- **Purpose**: Authentication via Firebase Admin SDK (token verification).
- **Key config (env)**: `Firebase__Type`, `Firebase__ProjectId`, `Firebase__PrivateKeyId`, `Firebase__PrivateKey`, `Firebase__ClientEmail`, `Firebase__ClientId`, `Firebase__AuthUri`, `Firebase__TokenUri`, `Firebase__AuthProviderX509CertUrl`, `Firebase__ClientX509CertUrl`, `Firebase__UniverseDomain`
- **Config class**: `JCertPreApplication.Domain/Configuration/FirebaseConfiguration.cs`
- **Usage**: `JCertPreApplication.Application/Features/Auth/AuthService.cs`, API controller `JCertPreApplication.API/Controllers/AuthController.cs`

### LiveKit
- **Purpose**: Real-time audio/video for livestream sessions.
- **Key config (env)**: `LiveKit__ApiKey`, `LiveKit__ApiSecret`, `LiveKit__ServerUrl`
- **Config class**: `JCertPreApplication.Domain/Configuration/LiveKitConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/LiveKit/LiveKitService.cs`
- **Contracts/Features**: `JCertPreApplication.Application/Contracts/ILiveKitService.cs`, `JCertPreApplication.Application/Features/Livestreams/LivestreamService.cs`
- **API**: `JCertPreApplication.API/Controllers/LiveKitController.cs`

### PayOS
- **Purpose**: Payment processing (create orders, verify callbacks).
- **Key config (env)**: `PayOS__ClientId`, `PayOS__ApiKey`, `PayOS__ChecksumKey`, `PayOS__ReturnUrl`, `PayOS__CancelUrl`
- **Config class**: `JCertPreApplication.Domain/Configuration/PayOSConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/PayOSService.cs`
- **Features**: `JCertPreApplication.Application/Features/Payment/PaymentService.cs`

### Brevo SMTP (Email)
- **Purpose**: Transactional email (welcome, reset password, notifications) via Brevo SMTP.
- **Key config (env)**: `Smtp__Host`, `Smtp__Port`, `Smtp__EnableSsl`, `Smtp__Username`, `Smtp__Password`, `Smtp__FromEmail`, `Smtp__FromName`, `Smtp__Timeout`
- **Notes**: Use Brevo SMTP settings (e.g., host `smtp-relay.brevo.com`, port `587`, username is API key).
- **Config class**: `JCertPreApplication.Domain/Configuration/SmtpConfiguration.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/Mail/MailService.cs`
- **Templates**: `JCertPreApplication.Persistence/Templates/*`

### Google Gemini (Generative AI)
- **Purpose**: AI assistance (content generation or evaluation within features).
- **Key config (env)**: `GeminiAI__ApiKey`, `GeminiAI__BaseUrl`, `GeminiAI__Model`, `GeminiAI__RateLimitRPM`, `GeminiAI__TimeoutSeconds`, `GeminiAI__MaxRetries`
- **Config class**: `JCertPreApplication.Domain/Configuration/GeminiConfiguration.cs`
- **Models**: `JCertPreApplication.Persistence/Models/GeminiModels.cs`
- **Implementation**: `JCertPreApplication.Persistence/Services/GeminiService.cs`
- **Docs**: `Documents/AI_INTEGRATION.md`

### PostgreSQL (Database)
- **Purpose**: Primary relational database for the application.
- **Key config (env)**: `ConnectionStrings__JCertPreDB`
- **DbContext**: `JCertPreApplication.Persistence/DatabaseContext/JCertPreDatabaseContext.cs`
- **Migrations**: `JCertPreApplication.Persistence/Migrations/*`

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

