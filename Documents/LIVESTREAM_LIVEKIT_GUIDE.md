# Livestream with LiveKit: Architecture and Implementation Guide

This document explains the livestream feature of JCertPre using the LiveKit platform. It begins with a concise overview of LiveKit capabilities used by the system, then details how the feature is implemented: lifecycle, APIs, client flows, error handling, and operations.

---

## Table of Contents
1. [LiveKit Overview (Tech Introduction)](#livekit-overview-tech-introduction)
   - [Why LiveKit](#why-livekit)
   - [Server SDK (RoomService)](#server-sdk-roomservice)
   - [Access Tokens & VideoGrants](#access-tokens--videogrants)
   - [Core Server Operations](#core-server-operations)
2. [Livestream Feature (Product Implementation)](#livestream-feature-product-implementation)
   - [Status Flow & Lifecycle](#status-flow--lifecycle)
   - [Background Service Automation](#background-service-automation)
   - [API Endpoints](#api-endpoints)
   - [Integration Flow](#integration-flow)
   - [Client Implementation](#client-implementation)
3. [Error Handling](#error-handling)
4. [Configuration & Data Model](#configuration--data-model)
5. [Best Practices](#best-practices)

---

## LiveKit Overview (Tech Introduction)

### Why LiveKit
LiveKit is a real-time communications platform for low-latency audio/video and data. JCertPre uses LiveKit to host interactive livestream sessions for online courses.

Advantages:
- Server-side control of rooms, participants, and permissions
- Fine-grained permissions via VideoGrants
- Scalable server cluster with SDK support across web, mobile, and backend

### Server SDK (RoomService)
We use the LiveKit .NET Server SDK (RoomService) to manage rooms and participants from the backend.

Initialize client (example):
```csharp
var roomServiceClient = new RoomServiceClient(
    "https://your-livekit-host.com",
    "YOUR_API_KEY",
    "YOUR_API_SECRET"
);
```

Core operations provided by RoomService:
- Create, list, get, delete rooms
- List and remove participants
- Mute/unmute participant tracks by Track SID
- Send data messages (text/JSON/binary)
- Update participant permissions dynamically

### Access Tokens & VideoGrants
Clients connect to LiveKit using JWT tokens signed by the server. Tokens embed VideoGrants that define permissions.

Common grants used:
- `RoomJoin` (required to join)
- `Room` (room name scope)
- `CanPublish`, `CanSubscribe`, `CanPublishData`
- `CanPublishSources` (camera, microphone, screen_share, screen_share_audio)
- `RoomAdmin` (moderation controls)

Typical presets:
- Viewer: subscribe-only
- Presenter: publish camera/mic
- Instructor/Moderator: publish + admin actions (kick/mute)

### Core Server Operations
- Token issuing with appropriate grants per role
- Room creation with timeouts and metadata
- Participant management (list/remove/mute/unmute)
- Data messaging for control/notifications

---

## Livestream Feature (Product Implementation)

### Status Flow & Lifecycle
```
SCHEDULED → LIVE → COMPLETED
```
- SCHEDULED: Planned session
- LIVE: Active session with LiveKit room available
- COMPLETED: Session ended and room removed

### Background Service Automation
- Runs every 5 minutes
- Transitions SCHEDULED → LIVE 15 minutes before start: creates room
- Transitions LIVE → COMPLETED after end time + 10 minutes: deletes room
- Room settings used:
  - EmptyTimeout = 24h
  - DepartureTimeout = 24h
  - MaxParticipants = 100
  - Metadata includes `livestreamId` and `courseId`

Example settings:
```csharp
var roomSettings = new RoomSettings
{
    EmptyTimeout = TimeSpan.FromHours(24),
    DepartureTimeout = TimeSpan.FromHours(24),
    MaxParticipants = 100,
    Metadata = $"{\"livestreamId\":\"{livestreamId}\",\"courseId\":\"{courseId}\"}"
};
```

### API Endpoints

#### Livestream Controller (`/api/livestreams`)
- POST `/api/livestreams`: Create livestream (admin/system)
- GET `/api/livestreams/{id}`: Get livestream by ID
- PUT `/api/livestreams/{id}`: Update livestream (admin/system)
- DELETE `/api/livestreams/{id}`: Delete livestream (admin/system)
- GET `/api/livestreams`: Filtered list (courseId, userId, date range, timetableFormat, paging)
- GET `/api/livestreams/{id}/join-token?userId={guid}`: Issue join token (role auto-detected)
- GET `/api/livestreams/{id}/can-join?userId={guid}`: Check join permission

#### LiveKit Controller (`/api/livekit`)
- GET `/api/livekit/token`: Generate generic token
- POST `/api/livekit/rooms`: Create room
- GET `/api/livekit/rooms`: List rooms
- GET `/api/livekit/rooms/{roomName}`: Get room info
- DELETE `/api/livekit/rooms/{roomName}`: Delete room
- GET `/api/livekit/rooms/{roomName}/participants`: List participants
- DELETE `/api/livekit/rooms/{roomName}/participants/{identity}`: Remove participant
- POST `/api/livekit/rooms/{roomName}/participants/{identity}/mute`: Mute
- POST `/api/livekit/rooms/{roomName}/participants/{identity}/unmute`: Unmute

### Integration Flow
1) Admin creates a livestream (scheduled time + duration)
2) Background service creates room 15 minutes before start and sets LIVE
3) User flow:
   - Check `/can-join` → if true
   - Get `/join-token` → connect to LiveKit using returned token
4) Instructor may moderate via LiveKit controller (mute/unmute, participant list)
5) Background service deletes room and marks COMPLETED after end

### Client Implementation
- Timetable view: `GET /api/livestreams?userId=...&timetableFormat=true`
- Join flow: `GET /{id}/can-join` → `GET /{id}/join-token` → connect to LiveKit SDK with token
- Role-aware UI: expose instructor controls only when participantRole = Instructor

---

## Error Handling
Common cases:
- Schedule conflict, invalid schedule time
- Forbidden join (not enrolled/not instructor or not LIVE)
- Not found (livestream/room)

Client pattern:
```typescript
try {
  const tokenResp = await fetch(`/api/livestreams/${id}/join-token?userId=${userId}`);
  if (!tokenResp.ok) throw tokenResp;
  const { token } = await tokenResp.json();
  // connect with token
} catch (err) {
  if (err.status === 403) showMessage('You cannot join this livestream');
  else if (err.status === 404) showMessage('Livestream not found');
  else showMessage('Failed to join livestream');
}
```

---

## Configuration & Data Model

### Environment Variables
```env
LIVEKIT_API_KEY=your_api_key
LIVEKIT_API_SECRET=your_secret
LIVEKIT_WS_URL=wss://your-livekit-server.com

CONNECTION_STRING=your_connection_string

LIVESTREAM_CHECK_INTERVAL_MINUTES=5
ROOM_EMPTY_TIMEOUT_HOURS=24
```

### Database Tables
- `Livestreams`: Core livestream data (scheduled time, duration, status)
- `Courses`: Course information
- `Enrollments`: Student course enrollments
- `CourseInstructors`: Instructor assignments (determines instructor permissions)

---

## Best Practices
- Check `canJoin` before issuing tokens; avoid exposing token when not LIVE
- Use least-privilege VideoGrants; avoid RoomAdmin unless necessary
- Set reasonable token TTL; refresh for long sessions
- Monitor background service logs and room lifecycle events
- Plan for scale: participant caps, concurrent sessions, server capacity
