using MeetUpHubV2.Entities.Enums; // Enumlar için (Namespace doğruysa)
using System; 
using System.Collections.Generic; 

namespace MeetUpHubV2.Frontend.Models
{
    // API'den dönen RoomDto'yu karşılamak için
    public class RoomDto
    {
        public int Id { get; set; }
        public RoomCategory Category { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int Capacity { get; set; }
        public bool IsFull { get; set; }
        public List<int>? UserIds { get; set; } 
        public DateTime? StartTime { get; set; } 
    }
}