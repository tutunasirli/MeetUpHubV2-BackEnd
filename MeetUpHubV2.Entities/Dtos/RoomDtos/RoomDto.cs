using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime? StartTime { get; set; }


    }
}
