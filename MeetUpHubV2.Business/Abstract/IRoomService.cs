using MeetUpHubV2.Entities.Enums;
using MeetUpHubV2.Entities.Dtos.RoomDtos; // Bu 'using' doğru
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IRoomService
    {
        // <<< DÜZELTİLDİ: 'RoomResponse' yerine 'RoomResponseDto'
        Task<RoomResponseDto> AddUserToAppropriateRoom(int userId, RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate);

        Task<RoomDto?> GetRoomById(int roomId);

        Task<List<RoomDto>> GetAllRooms();

        Task RemoveUserFromRoom(int userId, int roomId);

        Task DeleteRoom(int roomId);
    }
}