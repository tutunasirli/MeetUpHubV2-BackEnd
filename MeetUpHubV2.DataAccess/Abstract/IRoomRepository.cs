using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Abstract
{
    public interface IRoomRepository
    {
        Task<Room?> GetAvailableRoom(RoomCategory category, TimeSlot timeSlot, int capacity, string city, DateTime selectedDate);
        Task<Room> CreateRoom(RoomCategory category, TimeSlot timeSlot, int capacity, string city, DateTime selectedDate);
        Task<Room?> GetRoomById(int roomId);

        Task AddUserToRoom(int userId, int roomId);
        Task RemoveUserFromRoom(int userId, int roomId);
        Task<bool> IsUserInRoom(int userId, int roomId);
        Task<int> GetRoomUserCount(int roomId);
        Task DeleteRoom(Room room);
        Task<bool> IsRoomFull(int roomId);
        Task UpdateRoom(int roomId);
        Task<bool> IsUserInTimeSlot(int userId, TimeSlot timeSlot, DateTime selectedDate);

        Task<List<Room>> GetAllRooms();
    }
}
