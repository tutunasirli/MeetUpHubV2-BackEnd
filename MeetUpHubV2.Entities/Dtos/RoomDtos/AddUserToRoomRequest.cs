using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class AddUserToRoomRequest
    {
        public int UserId { get; set; }
        public RoomCategory Category { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int RoomCapacity { get; set; }
    }
}
