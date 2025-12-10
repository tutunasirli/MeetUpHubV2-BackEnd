using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Concrete
{
    public class RoomRepository : IRoomRepository
    {
        private readonly MeetUpHubV2DbContext _context;

        public RoomRepository(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetAvailableRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate)
        {
            return await _context.Rooms
                .Include(r => r.UserRooms)
                .FirstOrDefaultAsync(r =>
                    r.Category == category &&
                    r.TimeSlot == timeSlot &&
                    r.Capacity == capacity &&
                    r.SelectedDate.Date == selectedDate.Date &&
                    r.UserRooms.Count < r.Capacity);
        }

        public async Task<Room> CreateRoom(RoomCategory category, TimeSlot timeSlot, int capacity, DateTime selectedDate)
        {
            var newRoom = new Room
            {
                Category = category,
                TimeSlot = timeSlot,
                Capacity = capacity,
                SelectedDate = selectedDate.Date,
                IsFull = false,
                StartTime = DateTime.UtcNow
            };

            await _context.Rooms.AddAsync(newRoom);
            await _context.SaveChangesAsync();
            return newRoom;
        }

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

        public async Task<bool> IsUserInRoom(int userId, int roomId)
        {
            return await _context.UserRooms
                .AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        }

        public async Task<int> GetRoomUserCount(int roomId)
        {
            return await _context.UserRooms
                .CountAsync(ur => ur.RoomId == roomId);
        }

        public async Task<bool> IsRoomFull(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
                return true;

            var userCount = await GetRoomUserCount(roomId);
            return userCount >= room.Capacity;
        }

        public async Task UpdateRoom(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                var userCount = await GetRoomUserCount(roomId);
                room.IsFull = userCount >= room.Capacity;

                _context.Rooms.Update(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserInTimeSlot(int userId, TimeSlot timeSlot, DateTime selectedDate)
        {
            return await _context.UserRooms
                .Include(ur => ur.Room)
                .AnyAsync(ur =>
                    ur.UserId == userId &&
                    ur.Room.TimeSlot == timeSlot &&
                    ur.Room.SelectedDate.Date == selectedDate.Date);
        }

        public async Task DeleteRoom(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room?> GetRoomById(int roomId)
        {
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
