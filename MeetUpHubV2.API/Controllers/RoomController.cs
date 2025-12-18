using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using MeetUpHubV2.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MeetUpHubV2.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // === KULLANICININ ODAYA GİRMESİ ===
        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom(
            int userId,
            [FromBody] JoinRoomRequestDto request)
        {
            var result = await _roomService.AddUserToAppropriateRoom(
                userId,
                request.Category,
                request.TimeSlot,
                request.Capacity,
                request.City,
                request.SelectedDate
            );

            return Ok(result);
        }

        // === ODA DETAY ===
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            var room = await _roomService.GetRoomById(roomId);
            if (room == null)
                return NotFound("Oda bulunamadı.");

            return Ok(room);
        }

        // === TÜM ODALAR ===
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRooms();
            return Ok(rooms);
        }

        // === ODAYI SİL ===
        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            await _roomService.DeleteRoom(roomId);
            return Ok("Oda silindi.");
        }

        // === KULLANICI ÇIKAR ===
        [HttpPost("leave")]
        public async Task<IActionResult> LeaveRoom(int userId, int roomId)
        {
            await _roomService.RemoveUserFromRoom(userId, roomId);
            return Ok("Kullanıcı odadan çıkarıldı.");
        }
    }
}
