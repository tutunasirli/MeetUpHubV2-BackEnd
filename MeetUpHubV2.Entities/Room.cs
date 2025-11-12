using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic; // ICollection için eklendi
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUpHubV2.Entities
{
    public class Room
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public RoomCategory Category { get; set; }
        
        public DateTime Date { get; set; } // <<< HATA (CS0117) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        
        public TimeSlot TimeSlot { get; set; } // <<< HATA (CS0117) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        
        public int Capacity { get; set; } // <<< HATA (CS0117 & CS1061) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        
        public bool IsFull { get; set; } = false; // <<< HATA (CS0117 & CS1061) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        
        public DateTime? StartTime { get; set; } // <<< HATA (CS0117) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR

        // Navigation property (İlişki)
        // <<< HATA (CS1061) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        public virtual ICollection<UserRoom> UserRooms { get; set; }
    }
}