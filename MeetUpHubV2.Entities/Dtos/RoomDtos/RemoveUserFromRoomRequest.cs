using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.RoomDtos
{
    public class RemoveUserFromRoomRequest
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
