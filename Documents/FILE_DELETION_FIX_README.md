# File Deletion URL Parsing Fix

## Overview
This branch fixes the bug with file deletion URL parsing in the JCertPre-BE application. Previously, when updating resources that contained file URLs, the system would incorrectly extract the publicId from URLs, causing deletion failures.

## Problem
The original implementation in various services (UserService, CourseService, DocumentService, QuestionService) used different methods to extract file IDs from URLs, which were:
1. **Inconsistent**: Each service had its own URL parsing logic
2. **Error-prone**: Simple parsing methods that didn't handle Appwrite URL format correctly
3. **Not validated**: No validation of URL format or project ownership

Example problematic URL:
```
https://appwrite.zd-dev.xyz/v1/storage/buckets/688ac2b600184b292da2/files/6893fc12000dfb5332e0/view?project=688a4dd2001884283038
```

The file ID is `6893fc12000dfb5332e0` (after `/files/` and before `/view`).

## Solution
### 1. Enhanced IFileService Interface
Added new methods to `IFileService`:
```csharp
Task<FileDeletionResult> DeleteFileByUrlAsync(string fileUrl, CancellationToken cancellationToken = default);
string? ExtractPublicIdFromUrl(string fileUrl);
```

### 2. Created FileUrlParser Utility
A centralized utility class (`JCertPreApplication.Application.Utilities.FileUrlParser`) that provides:
- **Robust URL parsing** with regex patterns
- **Validation methods** for URL format and project ownership
- **Extensible design** for future storage providers
- **Error handling** with graceful fallbacks

Key methods:
- `ExtractAppwriteFileId(string fileUrl)`: Extracts file ID from Appwrite URLs
- `IsValidAppwriteUrl(string fileUrl)`: Validates URL format
- `IsFromExpectedProject(string fileUrl, string projectId)`: Validates project ownership
- `ExtractAppwriteBucketId(string fileUrl)`: Extracts bucket ID

### 3. Updated AppwriteFileService
Enhanced the service implementation with:
- `DeleteFileByUrlAsync()`: Deletes files using URLs with built-in validation
- `ExtractPublicIdFromUrl()`: Wrapper for the utility method
- **Enhanced error handling**: Better error messages and logging
- **Security validation**: Ensures URLs belong to the expected project

### 4. Updated Service Implementations
Modified all services that handle file deletion:

**UserService**: 
- Uses `DeleteFileByUrlAsync()` for avatar deletion during updates
- Removed custom `ExtractPublicIdFromUrl()` method

**CourseService**: 
- Uses `DeleteFileByUrlAsync()` for thumbnail deletion during updates  
- Removed custom `ExtractFileIdFromUrl()` method

**DocumentService**: 
- Enhanced deletion logic with URL-first approach, fallback to publicId
- Better error handling for file deletion failures

**QuestionService**: 
- Uses `DeleteFileByUrlAsync()` for audio file deletion during updates
- Removed custom `ExtractAppwritePublicId()` method

### 5. New API Endpoints
Added endpoints to `FileController`:
- `DELETE /api/files/delete/by-url`: Delete files using URLs
- `POST /api/files/extract-public-id`: Test endpoint for URL parsing

### 6. New DTOs
Created supporting DTOs:
- `DeleteResourceByUrlDto`: For URL-based deletion requests
- `ExtractPublicIdDto`: For testing URL parsing

## Files Changed
### Core Changes
- `IFileService.cs`: Added new interface methods
- `FileUrlParser.cs`: New utility class for URL parsing
- `AppwriteFileService.cs`: Enhanced implementation
- `FileController.cs`: New endpoints
- `DeleteResourceByUrlDto.cs`: New DTO
- `ExtractPublicIdDto.cs`: New DTO

### Service Updates
- `UserService.cs`: Updated avatar deletion logic
- `CourseService.cs`: Updated thumbnail deletion logic  
- `DocumentService.cs`: Enhanced file deletion logic
- `QuestionService.cs`: Updated audio file deletion logic

### Tests
- `FileUrlParserTests.cs`: Comprehensive unit tests for URL parsing

## Benefits
1. **Centralized Logic**: All URL parsing in one place
2. **Robust Validation**: Proper format and project validation
3. **Better Error Handling**: More informative error messages
4. **Extensible**: Easy to add support for other storage providers
5. **Consistent**: All services use the same parsing logic
6. **Secure**: Validates URLs belong to expected project

## Testing
The implementation includes comprehensive unit tests covering:
- Valid Appwrite URL parsing
- Invalid URL handling
- Project ownership validation
- Bucket ID extraction
- Edge cases and error scenarios

## Usage Examples
### Delete by URL (Recommended)
```csharp
var result = await _fileService.DeleteFileByUrlAsync(fileUrl);
if (!result.Success)
{
    // Handle deletion failure
    logger.LogWarning("Failed to delete file: {Error}", result.ErrorMessage);
}
```

### Extract Public ID for Testing
```csharp
var publicId = _fileService.ExtractPublicIdFromUrl(fileUrl);
if (string.IsNullOrEmpty(publicId))
{
    // Invalid URL format
}
```

## Migration Notes
- **Backward Compatible**: Existing `DeleteFileAsync(publicId)` method still works
- **Gradual Migration**: Services can be updated one by one
- **No Breaking Changes**: All existing functionality preserved

## Security Improvements
- URL validation ensures files belong to correct project
- Better error messages without exposing sensitive information
- Graceful handling of malformed URLs

This fix resolves the file deletion issues and provides a more robust, maintainable solution for file management operations.
