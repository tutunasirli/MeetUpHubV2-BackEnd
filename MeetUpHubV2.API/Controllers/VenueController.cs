using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using MeetUpHubV2.Entities.Dtos.VenueDtos;
using MeetUpHubV2.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MeetUpHubV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        private readonly IVenueService _venueService;
        public VenueController(IVenueService service)
        {
            _venueService = service;
        }
        [HttpPost("addVenue")]
        public async Task<IActionResult> AddVenue([FromBody] AddVenueDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var venue = new Venue
            {
                Name = request.Name,
                Category = request.Category,
                City=request.City,
                Location = request.Location,
            };

            await _venueService.AddAsync(venue);
            return Ok(venue);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venue = await _venueService.GetAllAsync();
            return Ok(venue);
        }
        [HttpGet("byId/{venueId}")]
        public async Task<IActionResult> GetVenueById(int venueId)
        {
            var venue = await _venueService.GetByIdAsync(venueId);
            if (venue == null)
                return NotFound("Bu ID ile eşleşen mekan bulunamadı.");

            return Ok(venue);
        }
        [HttpGet("byName")]
        public async Task<IActionResult> GetVenueByName([FromQuery] string name)
        {
            var venue = await _venueService.GetByNameAsync(name);
            if (venue == null)
                return NotFound("Bu isimle eşleşen mekan bulunamadı.");

            return Ok(venue);
        }

        [HttpGet("byCategory")]
        public async Task<IActionResult> GetVenueByCategory([FromQuery] RoomCategory category)
        {
            var venues = await _venueService.GetByCategory(category);
            if (venues == null || !venues.Any())
            {
                return NotFound("Bu kategoriye ait mekan bulunamadı.");
            }
            return Ok(venues);
        }
        [HttpGet("byCity")]
        public async Task<IActionResult> GetByCity([FromQuery] string city)
        {
            var venues = await _venueService.GetByCity(city);
            if (venues == null || !venues.Any())
                return NotFound("Bu şehirde mekan bulunamadı.");

            return Ok(venues);
        }


        [HttpDelete("delete/{venueId}")]
        public async Task<IActionResult> Delete(int venueId)
        {
            var venue = await _venueService.DeleteAsync(venueId);
            if (venue == null)
                return NotFound("Bu ID ile eşleşen mekan bulunamadı.");

            return Ok(venue);
        }
        [HttpPut("update/{venueId}")]
        public async Task<IActionResult> Update(int venueId, [FromBody] AddVenueDto request)
        {
            var venue = await _venueService.UpdateAsync(venueId, request.Name, request.Category,request.City, request.Location);
            if (venue == null)
                return NotFound("Bu ID ile eşleşen mekan bulunamadı.");

            return Ok(venue);
        }
    }

}
