using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;

namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class RoomDto
    {
        public int Id { get; set; }

        public RoomCategory Category { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public int Capacity { get; set; }
        public bool IsFull { get; set; }

        public List<int> UserIds { get; set; }

        // Yeni eklenen zorunlu alanlar (RoomManager'ın ihtiyaç duyduğu)
        public string City { get; set; }
        public DateTime SelectedDate { get; set; }

        public int UserCount => UserIds?.Count ?? 0;

        public DateTime? StartTime { get; set; }
    }
}
