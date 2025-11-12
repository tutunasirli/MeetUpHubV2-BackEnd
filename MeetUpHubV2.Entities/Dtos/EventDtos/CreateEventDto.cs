using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.EventDtos
{
    public class CreateEventDto
    {
        public int RoomId { get; set; }
        public int? VenueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
