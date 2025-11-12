using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities
{
    // Bu, User ve Room arasında çok-a-çok ilişkiyi sağlayan
    // 'join' (bağlantı) tablosudur.
    public class UserRoom
    {
        // Foreign Key (Yabancı Anahtar)
        public int UserId { get; set; }
        
        // Navigation Property (İlişki)
        // <<< HATA (CS1061 - DbContext) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        public virtual User User { get; set; } 

        // Foreign Key (Yabancı Anahtar)
        public int RoomId { get; set; }
        
        // Navigation Property (İlişki)
        // <<< HATA (CS1061 - DbContext & Repository) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        public virtual Room Room { get; set; }
    }
}