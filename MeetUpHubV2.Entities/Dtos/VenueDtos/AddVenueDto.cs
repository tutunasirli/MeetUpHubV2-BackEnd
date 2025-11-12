using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.VenueDtos
{
    public class AddVenueDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public RoomCategory Category { get; set; }
    }
}
