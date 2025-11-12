using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using Microsoft.EntityFrameworkCore; // <<< EKLENDİ (ToListAsync, FirstOrDefaultAsync, vb. için)
using System; // <<< EKLENDİ (DateTime için)
using System.Collections.Generic;
using System.Linq; // <<< EKLENDİ (Where, CountAsync, vb. için)
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Concrete
{
    // IRoomRepository kontratındaki TÜM metotları uyguluyoruz
    public class RoomRepository : IRoomRepository
    {
        private readonly MeetUpHubV2DbContext _context;

        public RoomRepository(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        // --- YENİ EKLENEN/DÜZELTİLEN METOTLAR (9 HATAYI ÇÖZER) ---

        // 1. GetAvailableRoom (EKSİKTİ)
        public async Task<Room?> GetAvailableRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate)
        {
            // Odayı, kullanıcı ilişkileriyle (UserRooms) birlikte çekiyoruz
            return await _context.Rooms
                .Include(r => r.UserRooms) 
                .FirstOrDefaultAsync(r =>
                    r.Category == category &&
                    r.TimeSlot == timeSlot &&
                    r.Capacity == capacity &&
                    r.Date.Date == selectedDate.Date && // Sadece tarihi karşılaştır
                    r.IsFull == false);
        }

        // 2. CreateRoom (İMZASI YANLIŞTI)
        public async Task<Room> CreateRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate)
        {
            var newRoom = new Room
            {
                Category = category,
                TimeSlot = timeSlot,
                Capacity = capacity,
                Date = selectedDate.Date, // Sadece tarihi kaydet
                IsFull = false,
                StartTime = DateTime.UtcNow // Odanın oluşturulma zamanı (veya RoomManager'da ayarlanabilir)
            };
            
            await _context.Rooms.AddAsync(newRoom);
            await _context.SaveChangesAsync();
            return newRoom;
        }

        // 3. AddUserToRoom (EKSİKTİ)
        public async Task AddUserToRoom(int userId, int roomId)
        {
            var userRoom = new UserRoom
            {
                UserId = userId,
                RoomId = roomId
            };
            await _context.UserRooms.AddAsync(userRoom);
            await _context.SaveChangesAsync();
        }

        // 4. RemoveUserFromRoom (EKSİKTİ)
        public async Task RemoveUserFromRoom(int userId, int roomId)
        {
            var userRoom = await _context.UserRooms
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
                
            if (userRoom != null)
            {
                _context.UserRooms.Remove(userRoom);
                await _context.SaveChangesAsync();
            }
        }

        // 5. IsUserInRoom (EKSİKTİ)
        public async Task<bool> IsUserInRoom(int userId, int roomId)
        {
            return await _context.UserRooms
                .AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        }

        // 6. GetRoomUserCount (EKSİKTİ)
        public async Task<int> GetRoomUserCount(int roomId)
        {
            return await _context.UserRooms
                .CountAsync(ur => ur.RoomId == roomId);
        }

        // 7. IsRoomFull (EKSİKTİ)
        public async Task<bool> IsRoomFull(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return true; // Oda yoksa, dolu kabul edip hatayı önle
            }
            
            var userCount = await GetRoomUserCount(roomId);
            return userCount >= room.Capacity;
        }

        // 8. UpdateRoom (İMZASI YANLIŞTI)
        // Bu metot, RoomManager'ın çağırması için 'IsFull' durumunu günceller
        public async Task UpdateRoom(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                var userCount = await GetRoomUserCount(roomId);
                room.IsFull = (userCount >= room.Capacity);
                
                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
            }
        }
        
        // 9. IsUserInTimeSlot (EKSİKTİ)
        public async Task<bool> IsUserInTimeSlot(int userId, TimeSlot timeSlot)
        {
            // Kullanıcının, seçtiği zaman diliminde başka bir odada olup olmadığını kontrol et
            return await _context.UserRooms
                .Include(ur => ur.Room) // Room tablosuna join yap
                .AnyAsync(ur => 
                    ur.UserId == userId && 
                    ur.Room.TimeSlot == timeSlot);
        }


        // --- MEVCUT DOĞRU METOTLAR (Aynı kalıyor) ---

        public async Task DeleteRoom(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room?> GetRoomById(int roomId)
        {
            // RoomManager'ın oylama için kullanıcı listesine ihtiyacı var,
            // bu yüzden 'Include(UserRooms)' ekliyoruz.
            return await _context.Rooms
                .Include(r => r.UserRooms)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms
                .Include(r => r.UserRooms)
                .ToListAsync();
        }
    }
}