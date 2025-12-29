using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema; // Gerek kalmadı
using Microsoft.AspNetCore.Identity; // BU SATIR EKLENDİ

namespace MeetUpHubV2.Entities
{
    // SINIF TANIMI DEĞİŞTİRİLDİ: IdentityUser'dan miras alıyor
    public class User : IdentityUser<int>
    {
        // [Key] ve Id alanı IdentityUser<int>'dan GELDİĞİ İÇİN SİLİNDİ.
        // Email alanı IdentityUser'dan GELDİĞİ İÇİN SİLİNDİ.
        // PhoneNumber alanı IdentityUser'dan GELDİĞİ İÇİN SİLİNDİ.

        // Password alanı, yerini IdentityUser'daki "PasswordHash" alanına
        // bıraktığı için GÜVENLİK nedeniyle SİLİNDİ.
        
        // --- SENİN ÖZEL ALANLARIN AYNEN KALIYOR ---
        
       // [Required]
       // [MaxLength(11)]
      //  public string TcNo { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Surname { get; set; }
        
        [Required]
        public DateTime BirthDate { get; set; }
        
        public DateTime RegistrationDate { get; set; }

        public UserDocument Document { get; set; }
        
        public string AccountStatus { get; set; }
        
        public virtual ICollection<UserRoom> UserRooms { get; set; }

        public string? About { get; set; }

    }
}