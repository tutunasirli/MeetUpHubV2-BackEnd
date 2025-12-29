using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities.Dtos.EventDtos;
using MeetUpHubV2.Entities;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace MeetUpHubV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Tüm Event’leri getir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        // Id’ye göre Event getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) // Metot adını daha açıklayıcı yaptım.
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
              
                return NotFound("Etkinlik bulunamadı.");
            }
            return Ok(ev);
        }
        //Kullanıcıların içinde olduğu eventleri getir.
        [HttpGet("{userId}/events")]
        public async Task<IActionResult> GetEventsForUser(int userId)
        {
            var events = await _eventService.GetEventsByUserIdAsync(userId);
            return Ok(events);
        }

        // Yeni Event oluştur
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDto createEventDto)
        {
           
            await _eventService.AddAsync(createEventDto);
            return StatusCode(201, "Etkinlik başarıyla oluşturuldu.");
        }

        // Event güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto updateDto)
        {
            // ID'leri kontrol etmek önemlidir.
            if (id != updateDto.Id)
            {
                return BadRequest("URL'deki ID ile gövdedeki ID uyuşmuyor.");
            }

            try
            {
                var result = await _eventService.UpdateAsync(updateDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                
                return NotFound(ex.Message);
            }
        }

        [Authorize]
[HttpGet("my")]
public async Task<IActionResult> GetMyEvents()
{
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
        return Unauthorized();

    int userId = int.Parse(userIdClaim);

    var events = await _eventService.GetEventsByUserIdAsync(userId);
    return Ok(events);
}

    }
}
