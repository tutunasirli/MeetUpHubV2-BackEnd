using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetUpHubV2.Entities
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Aktivite kategorisi
        public RoomCategory Category { get; set; }

        // Kullanıcıların seçtiği tarih (zorunlu)
        public DateTime SelectedDate { get; set; }

        // Sabah / Öğle / Akşam
        public TimeSlot TimeSlot { get; set; }

        // Oda kapasitesi
        public int Capacity { get; set; }

        // Doluluk bilgisi (isteğe bağlı, repository zaten kontrol ediyor)
        public bool IsFull { get; set; } = false;

        // Etkinlik başlangıç saati (opsiyonel)
        public DateTime? StartTime { get; set; }

        // Navigation property
        public virtual ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>();
    }
}
