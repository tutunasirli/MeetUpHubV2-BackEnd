namespace MeetUpHubV2.Entities.Dtos.RatingDtos
{
    public class CreateUserRatingDto
    {
        public int RatedUserId { get; set; }
        public int EventId { get; set; }
        public int Score { get; set; } // 1–5
    }
}
