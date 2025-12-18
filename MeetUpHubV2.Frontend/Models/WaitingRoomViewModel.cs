using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetUpHubV2.Frontend.Models
{
    public class WaitingRoomViewModel
    {
        public int RoomId { get; set; }
        public RoomCategory Category { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int Capacity { get; set; }
        public List<int> CurrentUserIds { get; set; } = new List<int>();
        public string CityName { get; set; } = "Bilinmiyor";
        public int TimeRemainingSeconds { get; set; } = 60;

        // Hesaplanmış özellikler
        public int CurrentUserCount => CurrentUserIds?.Count ?? 0;

        // Kullanıcıya gösterilecek kategori ismi (örnek formatlama)
        public string CategoryDisplayName
        {
            get
            {
                var name = Category.ToString();
                // CamelCase veya birleşik adları boşluk ekleyerek ayır
                return string.Concat(name.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
            }
        }

        // TimeSlot okunur string (örnek dönüşüm; enum değerlerine göre düzenleyebilirsin)
        public string TimeSlotDisplay
        {
            get
            {
                // Eğer TimeSlot enum'un "Eighteen_Twenty" gibi yazılıyorsa "_" yerine format uygula
                var s = TimeSlot.ToString();
                if (s.Contains("_"))
                {
                    // "18_20" => "18:00 - 20:00"
                    var parts = s.Split('_');
                    return parts.Length == 2 ? $"{parts[0]}:00 - {parts[1]}:00" : s;
                }
                // Default: CamelCase -> "Eighteen Twenty"
                return string.Concat(s.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
            }
        }
    }
}
