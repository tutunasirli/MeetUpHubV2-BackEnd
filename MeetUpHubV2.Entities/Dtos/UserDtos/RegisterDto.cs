
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace MeetUpHubV2.Entities.Dtos.UserDtos
{
    public class RegisterDto
    {
       // [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
       // [RegularExpression("^[0-9]*$", ErrorMessage = "Sadece rakam girilmelidir.")]
      //  public string TcNo { get; set; }
       [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public DateTime BirthDate { get; set; } // Doğrulama için gerekli
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
        //[Required(ErrorMessage = "Belge yüklenmelidir.")]
       // public IFormFile Document { get; set; }  // Adli sicil sorgulaması için yüklenecek belge.

    }

}
