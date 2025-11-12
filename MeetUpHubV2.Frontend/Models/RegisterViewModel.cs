using System;
using System.ComponentModel.DataAnnotations;

namespace MeetUpHubV2.Frontend.Models // Bu namespace satırının doğru olduğundan emin ol
{
    public class RegisterViewModel
    {
       // [Required(ErrorMessage = "TC Kimlik No alanı zorunludur.")] // Hata mesajı eklendi
      //<summary>
      /// [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
      // </summary>
      //  [RegularExpression("^[0-9]*$", ErrorMessage = "TC Kimlik No sadece rakam içermelidir.")]
      //  public string TcNo { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Doğum Tarihi alanı zorunludur.")]
        [DataType(DataType.Date)] // Tarayıcıda tarih seçici göstermeye yardımcı olur
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon Numarası alanı zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)] // Minimum şifre uzunluğu eklendi
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")] // Etikette görünecek isim
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}