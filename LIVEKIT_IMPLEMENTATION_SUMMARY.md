# Enhanced LiveKit Service Implementation

## Tóm tắt các thay đổi đã thực hiện

### 1. **Cập nhật ILiveKitService Interface**
File: `JCertPreApplication.Application\Contracts\ILiveKitService.cs`

**Thêm mới:**
- Enum `ParticipantRole` (Student, Instructor, Admin)
- Class `RoomSettings` và `RoomStatistics`
- 30+ methods cho quản lý comprehensive LiveKit operations

**Các nhóm chức năng:**
- **Token Management**: GenerateToken, GenerateAdminToken, GenerateRecordingToken
- **Room Management**: CreateRoom, ListRooms, GetRoom, DeleteRoom, UpdateRoomMetadata
- **Participant Management**: ListParticipants, RemoveParticipant, PromoteToInstructor, DemoteToStudent
- **Track Management**: MuteTrack, MuteParticipantAudio, UpdateSubscriptions
- **Data Management**: SendDataToRoom, BroadcastMessage, SendControlCommand
- **Webhook Processing**: ProcessWebhook, HandleRoomEvent
- **Utility Methods**: GetActiveRooms, GetInstructors, GetStudents, GetRoomStatistics

### 2. **Enhanced LiveKitService Implementation**
File: `JCertPreApplication.Persistence\Services\LiveKit\LiveKitService.cs`

**Tính năng chính:**
- **RoomServiceClient**: Tương tác trực tiếp với LiveKit server
- **WebhookReceiver**: Xử lý events từ LiveKit server
- **Role-based permissions**: Admin, Instructor, Student với quyền khác nhau
- **Comprehensive error handling**: Try-catch blocks và validation
- **Virtual event handlers**: Có thể override trong derived classes

**Key Methods Implemented:**
```csharp
// Token với roles và custom attributes
GenerateToken(roomName, identity, name, ParticipantRole.Instructor, TimeSpan.FromHours(2), attributes)

// Tạo room với settings
CreateRoomAsync(roomName, new RoomSettings { MaxParticipants = 50, EmptyTimeout = TimeSpan.FromMinutes(10) })

// Quản lý participants
PromoteToInstructorAsync(roomName, studentId)
MuteParticipantAudioAsync(roomName, participantId)

// Broadcasting và data
BroadcastMessageAsync(roomName, new { type = "announcement", message = "Class started!" })
SendControlCommandAsync(roomName, "start_recording", new { quality = "HD" })

// Room statistics
GetRoomStatisticsAsync(roomName) // Returns participant counts, creation time, recording status
```

### 3. **Enhanced LiveKitController API**
File: `JCertPreApplication.API\Controllers\LiveKitController.cs`

**New Endpoints:**
```
GET    /api/livekit/token                           - Generate participant token
GET    /api/livekit/admin-token                     - Generate admin token
POST   /api/livekit/rooms                          - Create room
GET    /api/livekit/rooms                          - List all rooms
GET    /api/livekit/rooms/{roomName}               - Get room details
DELETE /api/livekit/rooms/{roomName}               - Delete room
GET    /api/livekit/rooms/{roomName}/participants  - List participants
DELETE /api/livekit/rooms/{roomName}/participants/{identity} - Remove participant
POST   /api/livekit/rooms/{roomName}/participants/{identity}/promote - Promote to instructor
POST   /api/livekit/rooms/{roomName}/participants/{identity}/demote  - Demote to student
POST   /api/livekit/rooms/{roomName}/participants/{identity}/mute    - Mute participant
POST   /api/livekit/rooms/{roomName}/broadcast     - Broadcast message
GET    /api/livekit/rooms/{roomName}/statistics    - Get room statistics
POST   /api/livekit/webhook                        - Process LiveKit webhooks
```

### 4. **Domain Configuration Update**
File: `JCertPreApplication.Domain\Configuration\LiveKitConfiguration.cs`

**Thêm property:**
```csharp
public string ServerUrl { get; set; } = string.Empty;
```

### 5. **Package Dependencies**
- Thêm `Livekit.Server.Sdk.Dotnet` vào Application project để support interface types

## Cách sử dụng

### 1. **Tạo token cho participant**
```http
GET /api/livekit/token?roomName=class-101&role=Student
```

### 2. **Tạo room mới**
```http
POST /api/livekit/rooms
{
  "roomName": "math-class-grade-10",
  "maxParticipants": 30,
  "emptyTimeoutMinutes": 15,
  "metadata": "Math class for grade 10 students"
}
```

### 3. **Promote student thành instructor**
```http
POST /api/livekit/rooms/math-class-grade-10/participants/student123/promote
```

### 4. **Broadcast message tới toàn bộ class**
```http
POST /api/livekit/rooms/math-class-grade-10/broadcast
{
  "message": {
    "type": "announcement",
    "content": "Please turn on your cameras for attendance",
    "timestamp": "2025-01-27T10:30:00Z"
  }
}
```

### 5. **Lấy thống kê room**
```http
GET /api/livekit/rooms/math-class-grade-10/statistics
```

Response:
```json
{
  "roomName": "math-class-grade-10",
  "totalParticipants": 25,
  "instructorCount": 1,
  "studentCount": 24,
  "creationTime": "2025-01-27T10:00:00Z",
  "isRecording": true
}
```

## Lưu ý quan trọng

1. **Environment Configuration**: Cần thêm `LiveKit__ServerUrl` vào `.env` file
2. **Authentication**: Các endpoints admin yêu cầu authorization
3. **Error Handling**: Tất cả methods đều có proper error handling và validation
4. **Webhooks**: Server sẽ nhận events từ LiveKit để track room lifecycle
5. **Scalability**: Service support multiple rooms và thousands of participants

## Status
✅ **Build successful**
✅ **Application runs successfully**  
✅ **All enhanced features implemented**
✅ **Ready for testing with real LiveKit server**

Bây giờ bạn có thể test các chức năng LiveKit một cách comprehensive trước khi integrate vào các services khác!
