using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using MeetUpHubV2.Entities.Dtos.EventDtos;
using MeetUpHubV2.Entities.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUpHubV2.Business.Concrete
{
    public class RoomManager : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomNotificationService _notificationService;
        private readonly ILogger<RoomManager> _logger;
        private readonly IMemoryCache _cache;
        private readonly IVenueService _venueService;
        private readonly IEventService _eventService;

        public RoomManager(
            IRoomRepository roomRepository,
            IRoomNotificationService notificationService,
            ILogger<RoomManager> logger,
            IMemoryCache cache,
            IVenueService venueService,
            IEventService eventService)
        {
            _roomRepository = roomRepository;
            _notificationService = notificationService;
            _logger = logger;
            _cache = cache;
            _venueService = venueService;
            _eventService = eventService;
        }
public async Task<RoomDto?> GetRoomById(int roomId)
{
    var room = await _roomRepository.GetRoomById(roomId);
    if (room == null) return null;

    return new RoomDto
    {
        Id = room.Id,
        Category = room.Category,
        TimeSlot = room.TimeSlot,
        Capacity = room.Capacity,
        IsFull = room.IsFull,
        UserIds = room.UserRooms?.Select(x => x.UserId).ToList() ?? new List<int>(),
        StartTime = room.StartTime
    };
}

public async Task<List<RoomDto>> GetAllRooms()
{
    var rooms = await _roomRepository.GetAllRooms();

    return rooms.Select(room => new RoomDto
    {
        Id = room.Id,
        Category = room.Category,
        TimeSlot = room.TimeSlot,
        Capacity = room.Capacity,
        IsFull = room.IsFull,
        UserIds = room.UserRooms?.Select(x => x.UserId).ToList() ?? new List<int>(),
        StartTime = room.StartTime
    }).ToList();
}

public async Task RemoveUserFromRoom(int userId, int roomId)
{
    var room = await _roomRepository.GetRoomById(roomId);
    if (room == null)
        throw new Exception("Oda bulunamadı.");

    bool isUserInRoom = await _roomRepository.IsUserInRoom(userId, roomId);
    if (!isUserInRoom)
        throw new Exception("Kullanıcı bu odada değil.");

    await _roomRepository.RemoveUserFromRoom(userId, roomId);
    await _roomRepository.UpdateRoom(roomId);
}

public async Task DeleteRoom(int roomId)
{
    var room = await _roomRepository.GetRoomById(roomId);
    if (room != null)
    {
        await _roomRepository.DeleteRoom(room);
    }
}

        private string GetCacheKey(int roomId) => $"voting_session_{roomId}";

        public async Task<RoomResponseDto> AddUserToAppropriateRoom(
            int userId,
            RoomCategory category,
            TimeSlot timeSlot,
            int roomCapacity,
            string city,
            DateTime selectedDate)
        {
            _logger.LogInformation(
                "[RoomManager] AddUserToAppropriateRoom started for User:{UserId}, Category:{Category}",
                userId, category);

            try
            {
                bool isUserInTimeSlot =
                    await _roomRepository.IsUserInTimeSlot(userId, timeSlot, selectedDate);

                if (isUserInTimeSlot)
                {
                    return new RoomResponseDto
                    {
                        Success = false,
                        Message = "Aynı zaman diliminde başka bir odaya zaten katıldınız."
                    };
                }

                var existingRoom =
                    await _roomRepository.GetAvailableRoom(
                        category, timeSlot, roomCapacity, city, selectedDate);

                Room joinedOrCreatedRoom;
                bool isNewRoom = false;

                if (existingRoom != null)
                {
                    await _roomRepository.AddUserToRoom(userId, existingRoom.Id);
                    await _roomRepository.UpdateRoom(existingRoom.Id);
                    joinedOrCreatedRoom =
                        (await _roomRepository.GetRoomById(existingRoom.Id))!;
                }
                else
                {
                    isNewRoom = true;
                    var newRoom =
                        await _roomRepository.CreateRoom(
                            category, timeSlot, roomCapacity, city, selectedDate);

                    await _roomRepository.AddUserToRoom(userId, newRoom.Id);
                    joinedOrCreatedRoom =
                        (await _roomRepository.GetRoomById(newRoom.Id))!;
                }

                try
                {
                    bool roomIsNowFull =
                        await _roomRepository.IsRoomFull(joinedOrCreatedRoom.Id);

                    if (roomIsNowFull)
                    {
                        var votingSession =
                            new VotingSession(joinedOrCreatedRoom.Id.ToString());

                        _cache.Set(
                            GetCacheKey(joinedOrCreatedRoom.Id),
                            votingSession,
                            TimeSpan.FromMinutes(10));

                        var venues = await _venueService.GetAllAsync();
                        var timeSlotsForVoting =
                            new List<string> { "13:00", "14:00", "15:00" };

                        var votingOptions = new
                        {
                            Venues = venues,
                            TimeSlots = timeSlotsForVoting,
                            Duration = 30
                        };

                        var roomFromDb =
                            await _roomRepository.GetRoomById(joinedOrCreatedRoom.Id);

                        var userIdsInRoom = roomFromDb!.UserRooms!
                            .Select(x => x.UserId.ToString())
                            .ToList();

                        await _notificationService.SendMatchNotificationAsync(
                            userIdsInRoom,
                            joinedOrCreatedRoom.Id.ToString(),
                            votingOptions);

                        await _roomRepository.UpdateRoom(joinedOrCreatedRoom.Id);
                        _ = StartVotingTimer(joinedOrCreatedRoom.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "[RoomManager] Error during voting trigger for Room:{RoomId}",
                        joinedOrCreatedRoom.Id);
                }

                var roomDto = new RoomDto
                {
                    Id = joinedOrCreatedRoom.Id,
                    Category = joinedOrCreatedRoom.Category,
                    TimeSlot = joinedOrCreatedRoom.TimeSlot,
                    Capacity = joinedOrCreatedRoom.Capacity,
                    IsFull = joinedOrCreatedRoom.IsFull,
                    UserIds =
                        joinedOrCreatedRoom.UserRooms?
                        .Select(x => x.UserId).ToList()
                        ?? new List<int>(),
                    StartTime = joinedOrCreatedRoom.StartTime
                };

                return new RoomResponseDto
                {
                    Success = true,
                    Message = isNewRoom
                        ? "Yeni oda oluşturuldu."
                        : "Mevcut odaya katıldınız.",
                    Room = roomDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[RoomManager] Unhandled exception for User:{UserId}",
                    userId);

                return new RoomResponseDto
                {
                    Success = false,
                    Message = "Beklenmeyen bir hata oluştu."
                };
            }
        }

        private async Task StartVotingTimer(int roomId)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));

            var session =
                _cache.Get<VotingSession>(GetCacheKey(roomId));

            if (session == null) return;

            var winningVenueId =
                DetermineWinner(session.VenueVotes);

            var winningTimeSlot =
                DetermineWinner(session.TimeVotes);

            var newEventDto = new CreateEventDto
            {
                RoomId = roomId,
                VenueId = winningVenueId,
                Title = "Oylama Sonucu Etkinlik",
                Description = $"Saat: {winningTimeSlot}"
            };

            await _eventService.AddAsync(newEventDto);
            await _notificationService.SendVotingFinishedAsync(
                roomId.ToString(), newEventDto);

            _cache.Remove(GetCacheKey(roomId));
        }

        private int DetermineWinner(
            ConcurrentDictionary<int, int> votes)
        {
            if (votes == null || votes.IsEmpty) return 0;
            return votes.OrderByDescending(x => x.Value).First().Key;
        }

        private string? DetermineWinner(
            ConcurrentDictionary<string, int> votes)
        {
            if (votes == null || votes.IsEmpty) return null;
            return votes.OrderByDescending(x => x.Value).First().Key;
        }
    }
}
