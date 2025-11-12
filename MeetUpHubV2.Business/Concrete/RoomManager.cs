using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos;
using MeetUpHubV2.Entities.Dtos.RoomDtos;
using MeetUpHubV2.Entities.Dtos.EventDtos; 
using MeetUpHubV2.Entities.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly Dictionary<int, CancellationTokenSource> _roomDeletionTokens = new();

        public RoomManager(IRoomRepository roomRepository,
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

        private string GetCacheKey(string roomId) => $"voting_session_{roomId}";

        // IRoomService Metodu 1: (RoomResponseDto döndürür)
        public async Task<RoomResponseDto> AddUserToAppropriateRoom(int userId, RoomCategory category, TimeSlot timeSlot, int roomCapacity, DateTime selectedDate)
        {
            _logger.LogInformation("[RoomManager] AddUserToAppropriateRoom started for User:{UserId}, Category:{Category}", userId, category);
            try
            {
                bool isUserInTimeSlot = await _roomRepository.IsUserInTimeSlot(userId, timeSlot);
                if (isUserInTimeSlot)
                {
                    _logger.LogWarning("[RoomManager] User:{UserId} is already in another room for TimeSlot:{TimeSlot}", userId, timeSlot);
                    return new RoomResponseDto { Success = false, Message = "Aynı zaman diliminde başka bir odaya zaten katıldınız." };
                }

                var existingRoom = await _roomRepository.GetAvailableRoom(category, timeSlot, roomCapacity, selectedDate);
                Room? joinedOrCreatedRoom = null;
                bool isNewRoom = false;

                if (existingRoom != null)
                {
                    _logger.LogInformation("[RoomManager] Found existing room with ID:{RoomId}", existingRoom.Id);
                    await _roomRepository.AddUserToRoom(userId, existingRoom.Id);
                    await _roomRepository.UpdateRoom(existingRoom.Id);
                    joinedOrCreatedRoom = await _roomRepository.GetRoomById(existingRoom.Id);
                }
                else
                {
                    _logger.LogInformation("[RoomManager] No available room found. Creating new room for date {SelectedDate}", selectedDate.ToShortDateString());
                    isNewRoom = true;
                    joinedOrCreatedRoom = await _roomRepository.CreateRoom(category, timeSlot, roomCapacity, selectedDate);
                    _logger.LogInformation("[RoomManager] New room created with ID:{RoomId}. Adding User:{UserId}", joinedOrCreatedRoom.Id, userId);
                    await _roomRepository.AddUserToRoom(userId, joinedOrCreatedRoom.Id);
                    joinedOrCreatedRoom = await _roomRepository.GetRoomById(joinedOrCreatedRoom.Id);
                }

                if (joinedOrCreatedRoom == null)
                {
                    _logger.LogError("[RoomManager] Failed to get room details after join/create step.");
                    return new RoomResponseDto { Success = false, Message = "Oda bilgileri alınamadı." };
                }

                try
                {
                    bool roomIsNowFull = await _roomRepository.IsRoomFull(joinedOrCreatedRoom.Id);
                    _logger.LogInformation("[RoomManager] IsRoomFull check after adding user for Room:{RoomId}: {Result}", joinedOrCreatedRoom.Id, roomIsNowFull);

                    if (roomIsNowFull)
                    {
                        _logger.LogInformation("[RoomManager] Room is full. Starting voting process for Room:{RoomId}", joinedOrCreatedRoom.Id);
                        string stringRoomId = joinedOrCreatedRoom.Id.ToString();

                        var venues = await _venueService.GetAllAsync();
                        var timeSlotsForVoting = new List<string> { "13:00", "14:00", "15:00" };
                        var votingOptions = new { Venues = venues, TimeSlots = timeSlotsForVoting, Duration = 30 };

                        var room = await _roomRepository.GetRoomById(joinedOrCreatedRoom.Id);
                        if (room == null || room.UserRooms == null)
                        {
                            throw new InvalidOperationException("Oylama başlatılırken oda veya kullanıcı listesi bulunamadı.");
                        }

                        var userIdsInRoom = room.UserRooms
                                                .Select(ur => ur.UserId.ToString())
                                                .ToList();
                        
                        await _notificationService.SendMatchNotificationAsync(userIdsInRoom, stringRoomId, votingOptions);
                        
                        _logger.LogInformation("[RoomManager] Starting 30-second voting timer for Room:{RoomId}", joinedOrCreatedRoom.Id);
                        _ = StartVotingTimer(joinedOrCreatedRoom.Id, stringRoomId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[RoomManager] Error during match trigger/notification for Room:{RoomId}", joinedOrCreatedRoom.Id);
                }

                _logger.LogInformation("[RoomManager] AddUserToAppropriateRoom finished successfully for User:{UserId}, Room:{RoomId}", userId, joinedOrCreatedRoom.Id);
                
                // <<< --- DÖNÜŞÜM HATASI (CS0029) BURADA DÜZELTİLDİ --- >>>
                // Frontend'in beklediği 'RoomResponseDto' döndürülüyor
                return new RoomResponseDto
                {
                    Success = true,
                    Message = isNewRoom ? "Yeni oda oluşturuldu ve katıldınız." : "Mevcut odaya başarıyla katıldınız.",
                    Room = new RoomDto // Entity (Room) -> DTO (RoomDto)
                    {
                        Id = joinedOrCreatedRoom.Id,
                        Category = joinedOrCreatedRoom.Category,
                        TimeSlot = joinedOrCreatedRoom.TimeSlot,
                        Capacity = joinedOrCreatedRoom.Capacity,
                        IsFull = joinedOrCreatedRoom.IsFull,
                        UserIds = joinedOrCreatedRoom.UserRooms?.Select(ur => ur.UserId).ToList() ?? new List<int>(),
                        StartTime = joinedOrCreatedRoom.StartTime
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RoomManager] Unhandled exception in AddUserToAppropriateRoom for User:{UserId}", userId);
                return new RoomResponseDto { Success = false, Message = $"Odaya katılırken beklenmedik bir hata oluştu: {ex.Message}" };
            }
        }
        
        // IRoomService Metodu 2
        public async Task<RoomDto?> GetRoomById(int roomId)
        {
            _logger.LogInformation("[RoomManager] Getting room by ID:{RoomId}", roomId);
            var room = await _roomRepository.GetRoomById(roomId);
            if (room == null)
            {
                _logger.LogWarning("[RoomManager] Room not found for ID:{RoomId}", roomId);
                return null;
            }

            // Entity (Room) -> DTO (RoomDto)
            return new RoomDto
            {
                Id = room.Id,
                Category = room.Category,
                TimeSlot = room.TimeSlot,
                Capacity = room.Capacity,
                IsFull = room.IsFull,
                UserIds = room.UserRooms?.Select(ur => ur.UserId).ToList() ?? new List<int>(),
                StartTime = room.StartTime
            };
        }

        // IRoomService Metodu 3
        public async Task<List<RoomDto>> GetAllRooms()
        {
            _logger.LogInformation("[RoomManager] Getting all rooms.");
            var rooms = await _roomRepository.GetAllRooms();

            var roomDtos = rooms.Select(room => new RoomDto
            {
                Id = room.Id,
                Category = room.Category,
                TimeSlot = room.TimeSlot,
                Capacity = room.Capacity,
                IsFull = room.IsFull,
                UserIds = room.UserRooms?.Select(ur => ur.UserId).ToList() ?? new List<int>(),
                StartTime = room.StartTime
            }).ToList();

            _logger.LogInformation("[RoomManager] Found {Count} rooms.", roomDtos.Count);
            return roomDtos;
        }

        // IRoomService Metodu 4
        public async Task RemoveUserFromRoom(int userId, int roomId)
        {
            _logger.LogInformation("[RoomManager] Removing User:{UserId} from Room:{RoomId}", userId, roomId);
            var room = await _roomRepository.GetRoomById(roomId);
            if (room == null) throw new Exception("Oda bulunamadı.");
            var isInRoom = await _roomRepository.IsUserInRoom(userId, roomId);
            if (!isInRoom) throw new Exception("Kullanıcı bu odada değil.");
            await _roomRepository.RemoveUserFromRoom(userId, roomId);
            await _roomRepository.UpdateRoom(roomId);
        }

        // IRoomService Metodu 5
        public async Task DeleteRoom(int roomId)
        {
            _logger.LogInformation("[RoomManager] Deleting Room:{RoomId}", roomId);
            var room = await _roomRepository.GetRoomById(roomId);
            if (room != null)
            {
                await _roomRepository.DeleteRoom(room);
            }
        }
        
        // --- Oylama Zamanlayıcısı (Özel Metot) ---
        private async Task StartVotingTimer(int intRoomId, string stringRoomId)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                _logger.LogInformation("[RoomManager][Timer] 30 seconds finished for Room:{RoomId}", stringRoomId);

                var session = _cache.Get<VotingSession>(GetCacheKey(stringRoomId));
                if (session == null)
                {
                    _logger.LogWarning("[RoomManager][Timer] Voting session not found in cache for Room:{RoomId}.", stringRoomId);
                    return;
                }

                var winningVenueId = DetermineWinner(session.VenueVotes);
                var winningTimeSlot = DetermineWinner(session.TimeVotes);

                if (winningVenueId == 0 || winningTimeSlot == null)
                {
                    _logger.LogWarning("[RoomManager][Timer] Voting failed for Room:{RoomId}. Not enough votes.", stringRoomId);
                    return;
                }

                _logger.LogInformation("[RoomManager][Timer] Voting finished for Room:{RoomId}. Winner Venue:{VenueId}, Winner Time:{TimeSlot}",
                    stringRoomId, winningVenueId, winningTimeSlot);

                var room = await _roomRepository.GetRoomById(intRoomId);
                if (room == null)
                {
                     _logger.LogError("[RoomManager][Timer] Room object not found in DB for Room:{RoomId} when creating event.", stringRoomId);
                    return;
                }

                var newEventDto = new CreateEventDto
                {
                    RoomId = intRoomId,
                    VenueId = winningVenueId, 
                    Title = $"Buluşma: {room.Category}",
                    Description = $"Oylama sonucu oluşturulan etkinlik. Kazanan saat: {winningTimeSlot}"
                };

                await _eventService.AddAsync(newEventDto);
                _logger.LogInformation("[RoomManager][Timer] New event creation request sent for Room:{RoomId}", stringRoomId);
                
                await _notificationService.SendVotingFinishedAsync(stringRoomId, newEventDto);
                _cache.Remove(GetCacheKey(stringRoomId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RoomManager][Timer] Error in voting timer for Room:{RoomId}", stringRoomId);
            }
        }
        
        // --- Yardımcı Metotlar (TKey hataları (CS0019/CS0029) düzeltildi) ---
        private int DetermineWinner(ConcurrentDictionary<int, int> votes)
        {
            if (votes == null || votes.IsEmpty)
                return 0; // int için 'default' 0'dır

            var winner = votes.OrderByDescending(pair => pair.Value).FirstOrDefault();
            return winner.Key;
        }

        private string? DetermineWinner(ConcurrentDictionary<string, int> votes)
        {
            if (votes == null || votes.IsEmpty)
                return null; // string için 'default' null'dır

            var winner = votes.OrderByDescending(pair => pair.Value).FirstOrDefault();
            return winner.Key;
        }

        private DateTime ParseTimeSlot(string timeSlot)
        {
            try
            {
                return DateTime.Today.Add(TimeSpan.Parse(timeSlot));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse timeslot string: {TimeSlot}. Defaulting to midday.", timeSlot);
                return DateTime.Today.AddHours(12);
            }
        }
    }
}