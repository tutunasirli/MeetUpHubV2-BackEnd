using MeetUpHubV2.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class JoinRoomRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public RoomCategory Category { get; set; }

        [Required]
        public TimeSlot TimeSlot { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        [Required]
        public DateTime SelectedDate { get; set; }
    }
}
