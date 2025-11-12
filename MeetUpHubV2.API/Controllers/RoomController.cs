using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using Microsoft.AspNetCore.Authorization; // <<<=== EKLENDİ
using Microsoft.AspNetCore.Mvc;
using System; // Exception için eklendi
using System.Threading.Tasks; // Task için eklendi

namespace MeetUpHubV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // <<<=== İstersen tüm controller'ı koruyabilirsin ya da sadece metotları
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        

        // Bu metot MatchingController tarafından devralındı, belki kaldırılabilir?
        [HttpDelete("removeUser")] // POST yerine DELETE daha uygun olabilir
        [Authorize] // <<<=== Yetkilendirme eklendi (varsa)
        public async Task<IActionResult> RemoveUserFromRoom([FromBody] RemoveUserFromRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _roomService.RemoveUserFromRoom(request.UserId, request.RoomId);
                return Ok(new { message = "User removed from room." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- BU METOT WAITINGROOM İÇİN KULLANILACAK ---
        [HttpGet("{roomId}")]
        [Authorize] // <<<=== YETKİLENDİRME EKLENDİ
        public async Task<IActionResult> GetRoom(int roomId)
        {
            var room = await _roomService.GetRoomById(roomId);
            if (room == null)
                return NotFound(new { message = "Oda bulunamadı."}); // Daha açıklayıcı mesaj

            return Ok(room); // RoomDto dönüyor
        }
        // --- METODUN SONU ---

        [HttpDelete("{roomId}")]
        [Authorize] // <<<=== Yetkilendirme eklendi (Örn: Sadece admin silebilir?)
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            try
            {
                await _roomService.DeleteRoom(roomId);
                return Ok(new { message = "Oda başarıyla silindi." }); // Başarı mesajı güncellendi
            }
            catch (Exception ex)
            {
                // Silinecek oda bulunamadıysa NotFound dönmek daha iyi olabilir
                 if (ex.Message.Contains("bulunamadı")) // Basit kontrol
                 {
                     return NotFound(new { message = ex.Message });
                 }
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // TODO: Belki tüm odaları listeleyen bir endpoint eklenebilir?
        // [HttpGet]
        // [Authorize] 
        // public async Task<IActionResult> GetAllRooms() { ... }
    }
}