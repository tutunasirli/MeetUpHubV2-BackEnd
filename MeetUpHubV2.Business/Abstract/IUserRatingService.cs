using MeetUpHubV2.Entities.Dtos.RatingDtos;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IUserRatingService
    {
        Task RateUserAsync(int raterUserId, CreateUserRatingDto dto);
        Task<double> GetUserRatingAsync(int userId);
    }
}
