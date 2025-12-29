using System;
using System.Collections.Generic;

namespace MeetUpHubV2.Frontend.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; } = "";
        public List<UserEventViewModel> Events { get; set; } = new();
    }

    public class UserEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventTime { get; set; }
    }
}
