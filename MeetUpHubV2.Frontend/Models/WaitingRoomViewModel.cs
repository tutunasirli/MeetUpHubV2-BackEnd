using MeetUpHubV2.Entities.Enums;
using System.Collections.Generic;

namespace MeetUpHubV2.Frontend.Models
{
    public class WaitingRoomViewModel
    {
        public int RoomId { get; set; }
        public RoomCategory Category { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int Capacity { get; set; }
        public List<int> CurrentUserIds { get; set; } = new List<int>(); // Şimdilik sadece ID'ler
        public string CityName { get; set; } = "Bilinmiyor"; // Mekan bilgisi eklenince güncellenecek
        public int TimeRemainingSeconds { get; set; } = 60; // Başlangıç değeri, JS güncelleyecek

        // Kolay erişim için hesaplanmış özellikler
        public int CurrentUserCount => CurrentUserIds.Count;
        public string CategoryDisplayName => Category.ToString().Replace("Molasi", "").Replace("Deneyimi", "");
    }
}