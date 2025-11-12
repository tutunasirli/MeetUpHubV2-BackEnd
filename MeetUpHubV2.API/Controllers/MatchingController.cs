using MeetUpHubV2.Business.Abstract; // IRoomService için
using MeetUpHubV2.Entities.Dtos.RoomDtos; // JoinRoomRequestDto için
using Microsoft.AspNetCore.Authorization; // [Authorize] için
using Microsoft.AspNetCore.Mvc;
using System; // Exception için
using System.Security.Claims; // ClaimTypes için
using System.Threading.Tasks; // Task için
using System.ComponentModel.DataAnnotations; // Bu satırı ekle
namespace MeetUpHubV2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // <<<=== BU CONTROLLER'DAKİ TÜM METOTLAR GİRİŞ YAPMAYI GEREKTİRİR
    public class MatchingController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public MatchingController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // POST /api/matching/join
        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequestDto requestDto)
        {
            // Giriş yapmış kullanıcının ID'sini token'dan al
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID'si alınamadı."); // Token geçerli değilse veya ID yoksa
            }

            try
            {
                // Business katmanındaki servisi çağır
                var roomResponse = await _roomService.AddUserToAppropriateRoom(
                    userId, 
                    requestDto.Category, 
                    requestDto.TimeSlot, 
                    requestDto.Capacity,
                    requestDto.SelectedDate); // <<<=== EKLENDİ

                if (roomResponse.Success)
                {
                    // Başarılı ise oda bilgisini döndür
                    // Frontend bu bilgiyi kullanarak WaitingRoom sayfasına yönlendirecek
                    return Ok(roomResponse.Room); 
                }
                else
                {
                    // Başarısız ise (örn: oda dolu, kullanıcı başka odada) hata mesajını döndür
                    return BadRequest(new { message = roomResponse.Message });
                }
            }
            catch (InvalidOperationException ex) // Kullanıcı başka odadaysa RoomManager bunu fırlatıyordu
            {
                 return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Beklenmedik diğer hatalar için
                // Loglama yapılabilir
                return StatusCode(500, "Odaya katılırken beklenmedik bir hata oluştu.");
            }
        }

        // POST /api/matching/leave
        [HttpPost("leave")]
        public async Task<IActionResult> LeaveRoom([FromBody] LeaveRoomRequestDto requestDto) // Basit bir DTO tanımlayabiliriz
        {
             var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID'si alınamadı.");
            }

            try
            {
                await _roomService.RemoveUserFromRoom(userId, requestDto.RoomId);
                return Ok(new { message = "Odadan başarıyla ayrıldınız." });
            }
            catch(Exception ex)
            {
                 // Loglama yapılabilir
                 // Kullanıcıya daha spesifik hata mesajları gösterilebilir (Oda bulunamadı, kullanıcı odada değil vb.)
                 return BadRequest(new { message = $"Odadan ayrılırken bir hata oluştu: {ex.Message}" });
            }
        }
    }

    // LeaveRoom için basit DTO (Aynı dosyaya veya Dtos altına eklenebilir)
    public class LeaveRoomRequestDto 
    {
        [Required]
        public int RoomId { get; set; }
    }
}