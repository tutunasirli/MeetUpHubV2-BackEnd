using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities.Dtos.RatingDtos;

namespace MeetUpHubV2.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IUserRatingService _ratingService;

        public RatingController(IUserRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> RateUser(CreateUserRatingDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _ratingService.RateUserAsync(userId, dto);
            return Ok("Puanlama başarılı.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRating(int userId)
        {
            var rating = await _ratingService.GetUserRatingAsync(userId);
            return Ok(rating);
        }
    }
}
