using MeetUpHubV2.Entities.Enums;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IRoomService
    {
        // Kullanıcıyı uygun odaya ekle
        Task<RoomResponseDto> AddUserToAppropriateRoom(
            int userId,
            RoomCategory category,
            TimeSlot timeSlot,
            int capacity,
            string city,
            DateTime selectedDate
        );

        // Oda ID ile getir
        Task<RoomDto?> GetRoomById(int roomId);

        // Tüm odaları getir
        Task<List<RoomDto>> GetAllRooms();

        // Kullanıcıyı odadan çıkar
        Task RemoveUserFromRoom(int userId, int roomId);

        // Odayı sil
        Task DeleteRoom(int roomId);
    }
}
