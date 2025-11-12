using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities
{
    public class UserDocument
    {
        public int Id { get; set; } //Döküman id user içine eklenecek.

        // Foreign key
        public int? UserId { get; set; } //Döküman yükleyen kullanıcı id

        // Belge bilgileri
        public string FileName { get; set; } //Dosya adi
        public string FilePath { get; set; }//Dosya yolu
        public DateTime UploadedAt { get; set; }//Dosya yüklenme tarihi.

        // Navigation property --> bir entity'nin başka bir entity ile olan ilişkisini temsil eden C# property’sidir.
        public User User { get; set; }
        //Bu işlem kodda ilişkili verilere ulaşmayı kolaylaştırır.Entity frameworkün ilişkileri anlamasını sağlar.
    }

}
