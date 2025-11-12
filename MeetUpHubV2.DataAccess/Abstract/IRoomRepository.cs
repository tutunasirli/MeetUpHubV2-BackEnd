using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using System; // DateTime için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Abstract
{
    public interface IRoomRepository
    {
        // === İMZALAR GÜNCELLENDİ ===
        Task<Room?> GetAvailableRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate); // Tarih parametresi DAHİL
        Task<Room> CreateRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate); // Tarih parametresi DAHİL
        Task<Room?> GetRoomById(int roomId); // <<<=== RoomDto yerine Room? DÖNDÜRÜR
        // === GÜNCELLEME SONU ===
        
        Task AddUserToRoom(int userId, int roomId);
        Task RemoveUserFromRoom(int userId, int roomId);
        Task<bool> IsUserInRoom(int userId, int roomId);
        Task<int> GetRoomUserCount(int roomId);
        Task DeleteRoom(Room room);
        Task<bool> IsRoomFull(int roomId);
        Task UpdateRoom(int roomId);
        Task<bool> IsUserInTimeSlot(int userId, TimeSlot timeSlot); // Şimdilik sadece TimeSlot'a bakıyor
        Task<List<Room>> GetAllRooms();
    }
}