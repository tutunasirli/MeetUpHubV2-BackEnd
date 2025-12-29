using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetUpHubV2.DataAccess.Concrete
{
    public class UserRatingRepository : IUserRatingRepository
    {
        private readonly MeetUpHubV2DbContext _context;

        public UserRatingRepository(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int raterUserId, int ratedUserId, int eventId)
        {
            return await _context.UserRatings.AnyAsync(r =>
                r.RaterUserId == raterUserId &&
                r.RatedUserId == ratedUserId &&
                r.EventId == eventId);
        }

        public async Task AddAsync(UserRating rating)
        {
            _context.UserRatings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingAsync(int userId)
        {
            return await _context.UserRatings
                .Where(r => r.RatedUserId == userId)
                .AverageAsync(r => (double?)r.Score) ?? 0;
        }
    }
}
