using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.RatingDtos;

namespace MeetUpHubV2.Business.Concrete
{
    public class UserRatingManager : IUserRatingService
    {
        private readonly IUserRatingRepository _ratingRepo;
        private readonly IEventRepository _eventRepo;

        public UserRatingManager(
            IUserRatingRepository ratingRepo,
            IEventRepository eventRepo)
        {
            _ratingRepo = ratingRepo;
            _eventRepo = eventRepo;
        }

        public async Task RateUserAsync(int raterUserId, CreateUserRatingDto dto)
        {
            if (dto.Score < 1 || dto.Score > 5)
                throw new InvalidOperationException("Puan 1 ile 5 arasında olmalıdır.");

            if (raterUserId == dto.RatedUserId)
                throw new InvalidOperationException("Kendinize puan veremezsiniz.");

            var ev = await _eventRepo.GetByIdAsync(dto.EventId);
            if (ev == null)
                throw new InvalidOperationException("Etkinlik bulunamadı.");

            if (DateTime.UtcNow <= ev.EventTime)
                throw new InvalidOperationException("Etkinlik henüz bitmedi.");

            var alreadyRated = await _ratingRepo.ExistsAsync(
                raterUserId, dto.RatedUserId, dto.EventId);

            if (alreadyRated)
                throw new InvalidOperationException("Bu kullanıcıyı zaten puanladınız.");

            var rating = new UserRating
            {
                RaterUserId = raterUserId,
                RatedUserId = dto.RatedUserId,
                EventId = dto.EventId,
                Score = dto.Score
            };

            await _ratingRepo.AddAsync(rating);
        }

        public async Task<double> GetUserRatingAsync(int userId)
        {
            return await _ratingRepo.GetAverageRatingAsync(userId);
        }
    }
}
