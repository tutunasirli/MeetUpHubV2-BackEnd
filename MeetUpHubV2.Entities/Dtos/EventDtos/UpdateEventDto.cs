using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.EventDtos
{
    public class UpdateEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EventStatus Status { get; set; }
        public DateTime EventTime { get; set; }
        public string Hour { get; set; }
        public int? VenueId { get; set; } // Mekan sonradan değiştirilebilir
    }
}
