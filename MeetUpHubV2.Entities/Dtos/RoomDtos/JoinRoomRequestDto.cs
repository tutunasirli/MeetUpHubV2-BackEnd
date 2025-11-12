using MeetUpHubV2.Entities.Enums;
using System.ComponentModel.DataAnnotations; // Required için

namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class JoinRoomRequestDto
    {
        [Required]
        public RoomCategory Category { get; set; }

        [Required]
        public TimeSlot TimeSlot { get; set; }

        [Required]
        [Range(1, 10)] // Örnek: Minimum 1, Maksimum 10 kişilik odalar
        public int Capacity { get; set; }
        
[Required]
public DateTime SelectedDate { get; set; }
        // public int VenueId { get; set; } // Şimdilik mekanı kullanmıyoruz
    }
}