using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities.Dtos.ProfileDtos;

namespace MeetUpHubV2.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEventService _eventService;

        public ProfileController(
            IUserService userService,
            IEventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }

        // 🔐 Giriş yapan kullanıcının profili
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Geçersiz kullanıcı bilgisi.");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var events = await _eventService.GetEventsByUserIdAsync(userId);

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.RegistrationDate,
                user.About,
                Events = events
            });
        }

        // ✅ HAKKIMDA GÜNCELLEME (EKLENEN KISIM)
        [HttpPut("me/about")]
        public async Task<IActionResult> UpdateMyAbout([FromBody] UpdateAboutDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Geçersiz kullanıcı bilgisi.");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.About = dto.About?.Trim();

            await _userService.UpdateAboutAsync(user.Id, dto.About);


            return Ok(new { success = true });
        }
    }
}
