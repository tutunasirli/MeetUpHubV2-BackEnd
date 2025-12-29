using MeetUpHubV2.Entities;

namespace MeetUpHubV2.DataAccess.Abstract
{
    public interface IUserRatingRepository
    {
        Task<bool> ExistsAsync(int raterUserId, int ratedUserId, int eventId);
        Task AddAsync(UserRating rating);
        Task<double> GetAverageRatingAsync(int userId);
    }
}
