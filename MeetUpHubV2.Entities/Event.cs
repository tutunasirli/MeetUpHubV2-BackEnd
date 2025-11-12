using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities
{
    public class Event
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Venue ile ilişki (son aşamada seçilir)
        public int? VenueId { get; set; }
        public Venue Venue { get; set; }

        // Event'e özgü bilgiler
        public string Title { get; set; }
        public string Description { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Planned;
        public DateTime EventTime{ get; set; } //Daha sonra eklenecek
        public string Hour { get; set; } //Etkinlik Saati.



        // Room'dan kopyalanan bilgiler (performans için)
        public TimeSlot TimeSlot { get;  set; }
        public RoomCategory Category { get;  set; }
        public int Capacity { get;  set; }
        public List<User> Participants { get; set; } = new List<User>();
    }
}
