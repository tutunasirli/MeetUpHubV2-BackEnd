using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Entities.Dtos.UserDtos
{
    public class UserDto
    {
        //Get işlemlerinde bu Dto üzerinden veriler çekildek, buraya ekeldiğim alanlar get işlemlerinde görünecek alanlardır.
        //Daha sonra İd ile veri getirme işlemini de buraya çekeceğim.

        public int Id { get; set; }
        public string TcNo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string DocumentName { get; set; }
        public string FullName { get; set; }

    }
}
