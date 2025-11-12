namespace MeetUpHubV2.Frontend.Models
{
    // API'den dönen RoomResponse'u karşılamak için
    public class RoomResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public RoomDto? Room { get; set; } // RoomDto'yu da oluşturacağız
    }
}