namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class LivestreamJoinDto
    {
        public string Token { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime ScheduledDateTime { get; set; }
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
    }
}
