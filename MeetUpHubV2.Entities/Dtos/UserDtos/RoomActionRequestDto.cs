using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.UserDtos
{
    public class RoomActionRequestDto
    {
        public int UserId { get; set; }
        public int RoomId { get; set; }
    }
}
