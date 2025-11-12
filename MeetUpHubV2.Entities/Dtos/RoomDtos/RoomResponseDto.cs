namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class RoomResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public RoomDto? Room { get; set; }
    }
}
