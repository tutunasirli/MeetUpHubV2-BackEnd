using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetUpHubV2.Entities.Enums;

namespace MeetUpHubV2.Entities
{
    public class Venue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public RoomCategory Category { get; set; } //Tatlı Yemek Kahve yine buradan alınacak.
        public string City { get; set; }
        public string Location { get; set; }
        // Events ilişkisi
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
