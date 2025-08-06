# LiveKit .NET Server SDK - RoomService Documentation (Tiếng Việt)

## 1. Cài đặt và khởi tạo

```csharp
// Cài đặt NuGet package
// dotnet add package Livekit.Server.Sdk.Dotnet

using Livekit.Server.Sdk.Dotnet;

var roomServiceClient = new RoomServiceClient(
    "https://your-livekit-host.com",
    "YOUR_API_KEY", 
    "YOUR_API_SECRET"
);
```

## 2. Access Token và Authentication (Cực kỳ quan trọng)

### Tạo Access Token với đầy đủ thông tin

```csharp
using Livekit.Server.Sdk.Dotnet;
using System;

// Khởi tạo AccessToken với API key và secret
var accessToken = new AccessToken("YOUR_API_KEY", "YOUR_API_SECRET")
    .WithIdentity("user-unique-id")           // Định danh duy nhất (bắt buộc)
    .WithName("Tên hiển thị của user")        // Tên hiển thị (tùy chọn)
    .WithMetadata("{\"role\":\"moderator\"}")  // Metadata JSON (tùy chọn)
    .WithTtl(TimeSpan.FromHours(6));          // Thời gian sống token

// Cấu hình VideoGrants (quyền hạn trong phòng)
var videoGrants = new VideoGrants
{
    // === QUYỀN CƠ BẢN ===
    RoomJoin = true,                    // Cho phép tham gia phòng (bắt buộc)
    Room = "ten-phong-hop",            // Tên phòng được phép tham gia

    // === QUYỀN PUBLISH/SUBSCRIBE ===
    CanPublish = true,                 // Cho phép gửi video/audio
    CanSubscribe = true,               // Cho phép nhận video/audio từ người khác
    CanPublishData = true,             // Cho phép gửi data messages
    
    // === QUYỀN NÂNG CAO ===
    CanUpdateOwnMetadata = true,       // Cho phép tự cập nhật metadata
    Hidden = false,                    // true = ẩn danh (không xuất hiện trong danh sách)
    
    // === QUYỀN QUẢN TRỊ (CẨN THẬN) ===
    RoomAdmin = false,                 // Quyền quản trị phòng (kick người khác, mute, etc.)
    RoomCreate = false,                // Quyền tạo phòng mới
    RoomList = false,                  // Quyền liệt kê các phòng
    RoomRecord = false,                // Quyền sử dụng Egress (ghi video)
    IngressAdmin = false,              // Quyền sử dụng Ingress

    // === GIỚI HẠN NGUỒN PUBLISH ===
    CanPublishSources = new[] { "camera", "microphone" } // Chỉ cho phép camera và mic
};

accessToken.WithGrants(videoGrants);
string jwtToken = accessToken.ToJwt();
```

### Chi tiết đầy đủ về VideoGrants

| Quyền | Kiểu | Mô tả chi tiết |
|-------|------|----------------|
| RoomJoin | bool | Bắt buộc phải true để tham gia phòng |
| Room | string | Tên phòng được phép tham gia, bắt buộc nếu RoomJoin = true |
| CanPublish | bool | Cho phép publish tracks (video/audio). Mặc định: false |
| CanSubscribe | bool | Cho phép subscribe tracks từ người khác. Mặc định: true |
| CanPublishData | bool | Cho phép gửi data messages qua DataChannel |
| CanPublishSources | string[] | Giới hạn nguồn có thể publish: ["camera", "microphone", "screen_share", "screen_share_audio"] |
| CanUpdateOwnMetadata | bool | Cho phép participant tự cập nhật metadata của mình |
| Hidden | bool | Ẩn participant khỏi danh sách (dùng cho monitoring) |
| RoomAdmin | bool | Quyền mạnh: có thể kick, mute, update permissions của người khác |
| RoomCreate | bool | Cho phép tạo phòng mới |
| RoomList | bool | Cho phép liệt kê tất cả phòng |
| RoomRecord | bool | Cho phép sử dụng Egress service để ghi video |
| IngressAdmin | bool | Cho phép sử dụng Ingress service |

#### Ví dụ các loại token thực tế

**Token cho viewer (chỉ xem):**
```csharp
var viewerGrants = new VideoGrants
{
    RoomJoin = true,
    Room = "meeting-room-123",
    CanPublish = false,      // Không được publish
    CanSubscribe = true,     // Chỉ được xem
    CanPublishData = false   // Không được chat
};
```

**Token cho presenter (chỉ camera):**
```csharp
var presenterGrants = new VideoGrants
{
    RoomJoin = true,
    Room = "meeting-room-123", 
    CanPublish = true,
    CanSubscribe = true,
    CanPublishSources = new[] { "camera" }, // Chỉ camera, không mic
    CanPublishData = true
};
```

**Token cho moderator:**
```csharp
var moderatorGrants = new VideoGrants
{
    RoomJoin = true,
    Room = "meeting-room-123",
    CanPublish = true,
    CanSubscribe = true, 
    CanPublishData = true,
    RoomAdmin = true,        // Có thể kick/mute người khác
    CanUpdateOwnMetadata = true
};
```

## 3. RoomService APIs - Chi tiết đầy đủ

### A. Quản lý phòng (Room Management)

**CreateRoom(CreateRoomRequest request)**

```csharp
var createRoomRequest = new CreateRoomRequest
{
    Name = "meeting-room-123",           // Tên phòng (bắt buộc, duy nhất)
    EmptyTimeout = 300,                  // Tự động đóng sau 300 giây nếu rỗng
    MaxParticipants = 50,                // Tối đa 50 người
    Metadata = "{\"type\":\"meeting\",\"created_by\":\"admin\"}" // Metadata JSON
};

Room createdRoom = await roomServiceClient.CreateRoom(createRoomRequest);
Console.WriteLine($"Phòng '{createdRoom.Name}' được tạo với SID: {createdRoom.Sid}");
```

**Thuộc tính của Room object trả về:**
- Name: Tên phòng
- Sid: System ID duy nhất
- EmptyTimeout: Thời gian chờ trước khi đóng
- MaxParticipants: Số người tối đa
- CreationTime: Thời gian tạo (Unix timestamp)
- NumParticipants: Số người hiện tại
- Metadata: Dữ liệu tùy chỉnh

**ListRooms(ListRoomsRequest request)**

```csharp
// Lấy tất cả phòng
var allRoomsRequest = new ListRoomsRequest();
var allRooms = await roomServiceClient.ListRooms(allRoomsRequest);

// Lấy phòng cụ thể
var specificRequest = new ListRoomsRequest();
specificRequest.Names.Add("meeting-room-123");
specificRequest.Names.Add("another-room");
var specificRooms = await roomServiceClient.ListRooms(specificRequest);

foreach (var room in allRooms.Rooms)
{
    Console.WriteLine($"Room: {room.Name}, Participants: {room.NumParticipants}, Created: {room.CreationTime}");
}
```

**DeleteRoom(DeleteRoomRequest request)**

```csharp
var deleteRequest = new DeleteRoomRequest 
{ 
    Room = "meeting-room-123" 
};
await roomServiceClient.DeleteRoom(deleteRequest);
// Tất cả participants sẽ bị ngắt kết nối ngay lập tức
```

### B. Quản lý người tham gia (Participant Management)

**ListParticipants(ListParticipantsRequest request)**

```csharp
var listRequest = new ListParticipantsRequest 
{ 
    Room = "meeting-room-123" 
};
var response = await roomServiceClient.ListParticipants(listRequest);

foreach (var participant in response.Participants)
{
    Console.WriteLine($"Identity: {participant.Identity}");
    Console.WriteLine($"Name: {participant.Name}");
    Console.WriteLine($"Joined at: {participant.JoinedAt}");
    Console.WriteLine($"State: {participant.State}"); // JOINING, JOINED, DISCONNECTED
    Console.WriteLine($"Metadata: {participant.Metadata}");
    
    // Thông tin về tracks
    foreach (var track in participant.Tracks)
    {
        Console.WriteLine($"  Track SID: {track.Sid}, Type: {track.Type}, Source: {track.Source}");
    }
}
```

**Thuộc tính quan trọng của Participant:**
- Identity: Định danh duy nhất
- Name: Tên hiển thị
- State: Trạng thái kết nối
- JoinedAt: Thời gian tham gia
- Metadata: Dữ liệu tùy chỉnh
- Tracks: Danh sách tracks (video/audio) đang publish
- Permission: Quyền hạn hiện tại

**RemoveParticipant(RoomParticipantIdentity request)**

```csharp
var removeRequest = new RoomParticipantIdentity
{
    Room = "meeting-room-123",
    Identity = "user-to-remove"
};
await roomServiceClient.RemoveParticipant(removeRequest);
// Người dùng sẽ bị kick ngay lập tức và có thể tham gia lại nếu có token hợp lệ
```

**MutePublishedTrack(MuteRoomTrackRequest request)**

```csharp
// Trước tiên, lấy thông tin participant để biết Track SID
var participants = await roomServiceClient.ListParticipants(
    new ListParticipantsRequest { Room = "meeting-room-123" });

var targetParticipant = participants.Participants
    .FirstOrDefault(p => p.Identity == "noisy-user");

if (targetParticipant != null)
{
    var micTrack = targetParticipant.Tracks
        .FirstOrDefault(t => t.Source == TrackSource.Microphone);
    
    if (micTrack != null)
    {
        var muteRequest = new MuteRoomTrackRequest
        {
            Room = "meeting-room-123",
            Identity = "noisy-user",
            TrackSid = micTrack.Sid,
            Muted = true  // true = tắt, false = bật
        };
        await roomServiceClient.MutePublishedTrack(muteRequest);
    }
}
```

**UpdateParticipant(UpdateParticipantRequest request)**

```csharp
var updateRequest = new UpdateParticipantRequest
{
    Room = "meeting-room-123",
    Identity = "user-123",
    
    // Cập nhật metadata
    Metadata = "{\"role\":\"presenter\",\"verified\":true}",
    
    // Cập nhật quyền hạn trực tiếp trong phiên
    Permission = new ParticipantPermission
    {
        CanPublish = true,
        CanSubscribe = true,
        CanPublishData = false,    // Chặn chat
        CanUpdateOwnMetadata = false,
        Hidden = false
    }
};

ParticipantInfo updatedParticipant = await roomServiceClient.UpdateParticipant(updateRequest);
```

**Lưu ý quan trọng:** Khi bạn cập nhật quyền hạn qua `UpdateParticipant`, LiveKit server sẽ tự động gửi token mới cho client đó, và client sẽ nhận được thông báo về sự thay đổi quyền.

### C. Gửi dữ liệu (Data Messaging)

**SendData(SendDataRequest request)**

```csharp
using Google.Protobuf;
using System.Text;
using System.Text.Json;

// Gửi text message
var textMessage = new SendDataRequest
{
    Room = "meeting-room-123",
    Data = ByteString.CopyFrom("Hello from server!", Encoding.UTF8),
    Kind = DataPacket.Types.Kind.Reliable, // Đảm bảo đến nơi
};
await roomServiceClient.SendData(textMessage);

// Gửi JSON data cho người cụ thể
var jsonData = JsonSerializer.Serialize(new { 
    type = "notification", 
    message = "You have been promoted to moderator",
    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
});

var targetedMessage = new SendDataRequest
{
    Room = "meeting-room-123", 
    Data = ByteString.CopyFrom(jsonData, Encoding.UTF8),
    Kind = DataPacket.Types.Kind.Reliable,
};
// Gửi cho người cụ thể
targetedMessage.DestinationIdentities.Add("user-123");
targetedMessage.DestinationIdentities.Add("user-456");
await roomServiceClient.SendData(targetedMessage);

// Gửi binary data (ví dụ: file nhỏ)
byte[] binaryData = File.ReadAllBytes("small-file.png");
var binaryMessage = new SendDataRequest
{
    Room = "meeting-room-123",
    Data = ByteString.CopyFrom(binaryData),
    Kind = DataPacket.Types.Kind.Lossy, // Gửi nhanh, có thể mất gói
};
await roomServiceClient.SendData(binaryMessage);
```

**Hai loại Kind quan trọng:**
- Reliable: Giống TCP, đảm bảo dữ liệu đến nơi, có thể chậm hơn
- Lossy: Giống UDP, gửi nhanh, có thể mất gói, phù hợp cho dữ liệu real-time

## 4. Token Refresh và Permission Updates

**Cơ chế Token Refresh:**
- LiveKit server tự động làm mới token cho clients mỗi 10 phút
- Khi permissions thay đổi qua `UpdateParticipant`, server sẽ tạo token mới ngay lập tức
- Client không cần phải reconnect khi token được refresh

**Cập nhật quyền hạn động:**
```csharp
// Ví dụ: Nâng cấp viewer thành presenter trong lúc họp
var promoteToPresenter = new UpdateParticipantRequest
{
    Room = "meeting-room-123",
    Identity = "viewer-user",
    Permission = new ParticipantPermission
    {
        CanPublish = true,        // Từ false -> true
        CanSubscribe = true,
        CanPublishData = true,    // Được phép chat
        CanPublishSources = new[] { "camera", "microphone" }
    }
};
await roomServiceClient.UpdateParticipant(promoteToPresenter);
// User sẽ nhận được token mới và có thể bật camera/mic ngay lập tức
```

## 5. Error Handling và Best Practices

```csharp
try 
{
    var room = await roomServiceClient.CreateRoom(new CreateRoomRequest 
    { 
        Name = "test-room" 
    });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
{
    Console.WriteLine("Phòng đã tồn tại");
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated) 
{
    Console.WriteLine("API Key hoặc Secret không đúng");
}
catch (RpcException ex)
{
    Console.WriteLine($"Lỗi RPC: {ex.Status.Detail}");
}
```

**Lưu ý quan trọng:**
1. Bảo mật API Key/Secret: Không bao giờ expose API credentials ở client-side
2. Token Expiration: Luôn set TTL hợp lý, thường 1-24 giờ
3. Permissions: Chỉ cấp quyền tối thiểu cần thiết
4. Room Names: Sử dụng tên duy nhất, tránh collision
5. Error Handling: Luôn handle các RPC exceptions

---

Tài liệu này được tổng hợp và cập nhật dựa trên tài liệu chính thức của LiveKit và kinh nghiệm thực tế triển khai với .NET SDK.
