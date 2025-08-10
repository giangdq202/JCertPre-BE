# Livestream & LiveKit Integration Documentation

## Table of Contents
1. [Overview](#overview)
2. [Livestream Status Flow](#livestream-status-flow)
3. [Background Service](#background-service)
4. [API #### 5. Delete Room
```http
DELETE /api/livekit/rooms/{roomName}
```

#### 6. List Participants
```http
GET /api/livekit/rooms/{roomName}/participants
```

#### 7. Remove Participant
```http
DELETE /api/livekit/rooms/{roomName}/participants/{identity}
```

#### 8. Mute/Unmute Participantendpoints)
5. [Integration Flow](#integration-flow)
6. [Client Implementation Guide](#client-implementation-guide)
7. [Error Handling](#error-handling)
8. [Best Practices](#best-practices)

## Overview

The JCertPre application integrates with LiveKit to provide real-time livestreaming capabilities for online courses. The system automatically manages livestream lifecycle through a background service and provides comprehensive APIs for client interaction.

### Key Components
- **Livestream Entity**: Manages scheduled sessions
- **LiveKit Integration**: Real-time communication platform
- **Background Service**: Automatic lifecycle management
- **Role-based Permissions**: Student/Instructor access control

## Livestream Status Flow

### Status Transitions
```
SCHEDULED → LIVE → COMPLETED
```

#### Status Definitions
- **SCHEDULED**: Livestream is planned but not yet started
- **LIVE**: Livestream is currently active with LiveKit room available
- **COMPLETED**: Livestream has ended and room is closed

### Automatic Transitions

#### SCHEDULED → LIVE
- **Trigger**: 15 minutes before scheduled time
- **Action**: Background service creates LiveKit room
- **Room Settings**: 24-hour empty and departure timeout, max 100 participants
- **Access**: Users can join the room

#### LIVE → COMPLETED  
- **Trigger**: After scheduled end time + 10 minutes buffer (scheduledDateTime + durationMinutes + 10 minutes)
- **Action**: Background service deletes LiveKit room
- **Access**: Room becomes unavailable

## Background Service

### LivestreamStatusBackgroundService
- **Frequency**: Checks every 5 minutes
- **Purpose**: Automatic livestream lifecycle management
- **Features**:
  - Creates rooms 15 minutes before start time
  - Sets 24-hour empty and departure timeout to prevent premature closure
  - Deletes rooms after end time
  - Handles edge cases (already expired livestreams)
  - Comprehensive logging

### Room Management
```csharp
// Room creation with long timeout
var roomSettings = new RoomSettings
{
    EmptyTimeout = TimeSpan.FromHours(24), // Prevent auto-close when empty
    DepartureTimeout = TimeSpan.FromHours(24), // Prevent auto-close when all participants leave
    MaxParticipants = 100,
    Metadata = $"{{\"livestreamId\":\"{livestreamId}\",\"courseId\":\"{courseId}\"}}"
};
```

## API Endpoints

### Livestream Controller (`/api/livestreams`)

#### 1. Create Livestream (Admin/System Only)
```http
POST /api/livestreams
Content-Type: application/json

{
    "courseId": "guid",
    "description": "string",
    "scheduledDateTime": "2025-08-10T15:00:00Z",
    "durationMinutes": 90
}
```

**Response**: `201 Created` with livestream details
**Authorization**: Admin/System level access required

#### 2. Get Livestream by ID
```http
GET /api/livestreams/{id}
```

**Response**: Livestream details with calculated fields

#### 3. Update Livestream (Admin/System Only)
```http
PUT /api/livestreams/{id}
Content-Type: application/json

{
    "description": "string",
    "scheduledDateTime": "2025-08-10T16:00:00Z",
    "durationMinutes": 120
}
```

**Validation**: 
- Future schedule time only
- No scheduling conflicts
- Only allows updates for non-LIVE streams
**Authorization**: Admin/System level access required

#### 4. Delete Livestream (Admin/System Only)
```http
DELETE /api/livestreams/{id}
```

**Response**: `204 No Content`
**Authorization**: Admin/System level access required

#### 5. Get Livestreams (with filtering)
```http
GET /api/livestreams?courseId={guid}&userId={guid}&startDate={date}&endDate={date}&timetableFormat={bool}&pageIndex={int}&pageSize={int}
```

**Filtering Options**:
- `courseId`: Get livestreams for specific course
- `userId`: Get user's livestreams (as student or instructor based on role)
- `timetableFormat=true`: Return timetable view with join permissions
- Date range filtering with pagination

#### 6. Generate Join Token
```http
GET /api/livestreams/{id}/join-token?userId={guid}
```

**Requirements**:
- Livestream must be LIVE
- User must have permission (enrolled as student OR assigned as instructor)

**Response**:
```json
{
    "token": "jwt-token",
    "roomName": "livestream-id",
    "participantRole": "Student|Instructor"
}
```
*Note: Role is automatically determined based on user's relationship to the course*

#### 7. Check Join Permission
```http
GET /api/livestreams/{id}/can-join?userId={guid}
```

**Response**:
```json
{
    "canJoin": true
}
```

### LiveKit Controller (`/api/livekit`)

#### 1. Generate Access Token
```http
GET /api/livekit/token?roomName={string}&participantIdentity={string}&participantName={string}&role={Student|Instructor}
```

**Parameters**:
- `roomName`: Required - room to join
- `participantIdentity`: Optional - defaults to authenticated user ID
- `participantName`: Optional - display name
- `role`: Optional - defaults to Student

#### 2. Create Room
```http
POST /api/livekit/rooms
Content-Type: application/json

{
    "roomName": "string",
    "emptyTimeoutMinutes": 60,
    "departureTimeoutMinutes": 60,
    "maxParticipants": 100,
    "metadata": "string"
}
```

#### 3. List All Rooms
```http
GET /api/livekit/rooms
```

**Response**: Array of room objects with details like name, participants count, creation time, etc.

#### 4. Get Room Info
```http
GET /api/livekit/rooms/{roomName}
```

#### 5. Delete Room
```http
DELETE /api/livekit/rooms/{roomName}
```

#### 5. List Participants
```http
GET /api/livekit/rooms/{roomName}/participants
```

#### 6. Remove Participant
```http
DELETE /api/livekit/rooms/{roomName}/participants/{identity}
```

#### 7. Mute/Unmute Participant
```http
POST /api/livekit/rooms/{roomName}/participants/{identity}/mute
POST /api/livekit/rooms/{roomName}/participants/{identity}/unmute
```

## Integration Flow

### Complete Livestream Flow

#### 1. Admin/System Creates Livestream
```http
POST /api/livestreams
{
    "courseId": "course-guid",
    "description": "React Advanced Concepts",
    "scheduledDateTime": "2025-08-10T15:00:00Z",
    "durationMinutes": 90
}
```
*Note: Only admin/system can create livestreams. Instructors and students can only join existing livestreams.*

#### 2. Background Service Auto-Start (15 min before)
- Service detects livestream should start
- Creates LiveKit room with livestream ID as room name
- Updates status to LIVE
- Students can now join

#### 3. Instructor/Student Joins Livestream
```http
# Check if can join
GET /api/livestreams/{id}/can-join?userId={user-id}

# Get join token if allowed (with appropriate role)
GET /api/livestreams/{id}/join-token?userId={user-id}

# Connect to LiveKit with token (role automatically determined)
```

#### 4. Instructor Manages Session (Using LiveKit Controller directly)
```http
# Mute disruptive student using LiveKit API
POST /api/livekit/rooms/{livestream-id}/participants/{student-identity}/mute

# Check participants
GET /api/livekit/rooms/{livestream-id}/participants
```

#### 5. Background Service Auto-End (after duration)
- Service detects livestream ended
- Deletes LiveKit room
- Updates status to COMPLETED
- All participants (instructors and students) automatically disconnected

### User Timetable Integration
```http
GET /api/livestreams?userId={guid}&timetableFormat=true
```

**Response includes**:
- User's enrolled livestreams (as student) or assigned livestreams (as instructor)
- Permission status (`canJoin`)
- User role in each course
- Time status helpers

## Client Implementation Guide

### Frontend Integration Steps

#### 1. Display User Timetable
```typescript
// Get user's livestream timetable (works for both students and instructors)
const response = await fetch(`/api/livestreams?userId=${userId}&timetableFormat=true`);
const timetable = await response.json();

// Display with status indicators
timetable.forEach(livestream => {
    console.log(`${livestream.courseName}: ${livestream.timeStatus}`);
    console.log(`User role: ${livestream.userRole}`); // Student or Instructor
    if (livestream.canJoin && livestream.isLive) {
        showJoinButton(livestream);
    }
});
```

#### 2. Join Livestream (Student or Instructor)
```typescript
async function joinLivestream(livestreamId: string, userId: string) {
    // Check permission first
    const canJoinResponse = await fetch(`/api/livestreams/${livestreamId}/can-join?userId=${userId}`);
    const { canJoin } = await canJoinResponse.json();
    
    if (!canJoin) {
        throw new Error('Cannot join this livestream');
    }
    
    // Get join token (role automatically determined by system)
    const tokenResponse = await fetch(`/api/livestreams/${livestreamId}/join-token?userId=${userId}`);
    const { token, roomName, participantRole } = await tokenResponse.json();
    
    // Connect to LiveKit with appropriate permissions
    const room = new Room();
    await room.connect(LIVEKIT_URL, token);
    
    return { room, participantRole }; // participantRole will be 'Student' or 'Instructor'
}
```

#### 3. Instructor Controls (Using LiveKit API directly)
```typescript
// Use LiveKit API directly for participant management
async function muteParticipant(roomName: string, participantIdentity: string) {
    await fetch(`/api/livekit/rooms/${roomName}/participants/${participantIdentity}/mute`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${authToken}` // Must be instructor's token
        }
    });
}

async function unmuteParticipant(roomName: string, participantIdentity: string) {
    await fetch(`/api/livekit/rooms/${roomName}/participants/${participantIdentity}/unmute`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${authToken}` // Must be instructor's token
        }
    });
}

// Check if current user is instructor before showing controls
function showInstructorControls(participantRole: string) {
    if (participantRole === 'Instructor') {
        // Show mute/unmute buttons, participant management, etc.
        document.getElementById('instructor-controls').style.display = 'block';
    }
}
```

### Mobile Implementation
- Use LiveKit mobile SDKs (iOS/Android)
- Same API endpoints for token generation
- Handle automatic disconnection on COMPLETED status

## Error Handling

### Common Error Scenarios

#### 1. Schedule Conflicts
```json
{
    "error": "SCHEDULE_CONFLICT",
    "message": "There is already a livestream scheduled at this time for this course"
}
```

#### 2. Permission Denied
```json
{
    "error": "FORBIDDEN",
    "message": "You don't have permission to join this livestream or the livestream is not currently live"
}
```

#### 3. Livestream Not Found
```json
{
    "error": "NOT_FOUND",
    "message": "Livestream not found"
}
```

#### 4. Invalid Schedule Time
```json
{
    "error": "INVALID_SCHEDULE_TIME", 
    "message": "Scheduled time must be in the future"
}
```

### Client Error Handling
```typescript
try {
    const token = await getLivestreamToken(livestreamId, userId);
} catch (error) {
    if (error.status === 403) {
        showMessage('You cannot join this livestream');
    } else if (error.status === 404) {
        showMessage('Livestream not found');
    } else {
        showMessage('Failed to join livestream');
    }
}
```

## Best Practices

### For Developers

#### 1. Status Checking
- Always check `canJoin` before attempting to join
- Use timetable endpoint for comprehensive status
- Handle automatic disconnections gracefully

#### 2. Token Management
- Tokens have expiration times
- Regenerate tokens for long sessions
- Store tokens securely

#### 3. Real-time Updates
- Poll livestream status for UI updates
- Listen to LiveKit events for participant changes
- Handle network disconnections

#### 4. Performance
- Cache course and user permissions
- Use pagination for large timetables
- Optimize video quality based on connection

### For Administrators

#### 1. Livestream Management
- Only admins can create/update/delete livestreams
- Allow buffer time between sessions
- Validate instructor availability
- Consider timezone handling

#### 2. Monitoring
- Monitor background service logs
- Track room creation/deletion
- Alert on service failures

#### 3. Scaling
- Configure LiveKit cluster for high load
- Monitor room participant limits
- Plan for concurrent sessions

### For Instructors

#### 1. Joining Sessions
- Instructors join livestreams like students but with elevated permissions
- Test connectivity before live sessions
- Have backup communication channels

#### 2. Session Management
- Use instructor permissions to manage participants
- Monitor participant engagement
- Handle disruptive behavior with mute controls

#### 3. Content Delivery
- Prepare materials in advance
- Use screen sharing effectively
- Engage with chat/participants

## Configuration Requirements

### Environment Variables
```env
# LiveKit Configuration
LIVEKIT_API_KEY=your_api_key
LIVEKIT_API_SECRET=your_secret
LIVEKIT_WS_URL=wss://your-livekit-server.com

# Database
CONNECTION_STRING=your_connection_string

# Background Service
LIVESTREAM_CHECK_INTERVAL_MINUTES=5
ROOM_EMPTY_TIMEOUT_HOURS=24
```

### Database Tables
- `Livestreams`: Core livestream data (managed by admin/system)
- `Courses`: Course information
- `Enrollments`: Student course enrollments
- `CourseInstructors`: Instructor assignments (determines instructor permissions)

This documentation provides a comprehensive guide for implementing and using the livestream functionality with LiveKit integration in the JCertPre application. Note that livestream creation/management is restricted to admin/system level, while instructors and students can join livestreams with their respective permissions.
