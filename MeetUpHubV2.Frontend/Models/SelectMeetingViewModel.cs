using MeetUpHubV2.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System; // DateTime? için eklendi
using System.ComponentModel.DataAnnotations; // Required ve DataType için eklendi

namespace MeetUpHubV2.Frontend.Models
{
    public class SelectMeetingViewModel
    {
        [Required] // Kategori bilgisi Controller'dan gelecek
        public RoomCategory Category { get; set; }

        [Required(ErrorMessage = "Lütfen buluşma zamanını seçin.")]
        public TimeSlot? TimeSlot { get; set; } // Seçilmediyse null olabilir

        [Required(ErrorMessage = "Lütfen grup büyüklüğünü seçin.")]
        [Range(1, 10, ErrorMessage = "Geçersiz grup büyüklüğü.")]
        public int? Capacity { get; set; } // Seçilmediyse null olabilir

        [Required(ErrorMessage = "Lütfen şehir seçin.")]
        public string? City { get; set; } // Seçilmediyse null olabilir

        [Required(ErrorMessage = "Lütfen buluşma tarihini seçin.")]
        [DataType(DataType.Date)] // Tarayıcıya bunun bir tarih olduğunu belirtir
        [Display(Name = "Buluşma Tarihi")] // Label'da görünecek isim
        public DateTime? SelectedDate { get; set; } // Tarih seçimi için
    }
}