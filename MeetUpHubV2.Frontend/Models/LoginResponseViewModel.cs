using System;

namespace MeetUpHubV2.Frontend.Models // Namespace'in doğru olduğundan emin ol
{
    // Login API'sinden dönen cevabı tutmak için
    public class LoginResponseViewModel
    {
        public string? Token { get; set; } // Token null olabilir diye ? ekledik
        public DateTime Expiration { get; set; }
    }
}