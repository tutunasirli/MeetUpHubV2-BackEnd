using System;

namespace MeetUpHubV2.Entities
{
    public class UserRating
    {
        public int Id { get; set; }

        public int RaterUserId { get; set; }
        public int RatedUserId { get; set; }
        public int EventId { get; set; }

        public int Score { get; set; } // 1–5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
