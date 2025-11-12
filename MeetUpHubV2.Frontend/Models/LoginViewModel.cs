using System.ComponentModel.DataAnnotations;

namespace MeetUpHubV2.Frontend.Models // Namespace'in doğru olduğundan emin ol
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}