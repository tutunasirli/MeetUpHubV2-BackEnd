using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.UserDtos
{
    public class UpdatePasswordDto
    {
        //Sadece şifre güncellenecek.
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
