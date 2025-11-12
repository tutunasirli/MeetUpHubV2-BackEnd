using Azure.Core;
using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.EventDtos;
using MeetUpHubV2.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace MeetUpHubV2.Business.Concrete
{
    public class EventManager : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IVenueRepository _venueRepository;

        public EventManager(
       IEventRepository eventRepository,
       IRoomRepository roomRepository,
       IVenueRepository venueRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
            _venueRepository = venueRepository;
        }

        public async Task AddAsync(CreateEventDto createEventDto)
        {
            var room = await _roomRepository.GetRoomById(createEventDto.RoomId);
            if (room == null) throw new InvalidOperationException("Oda bulunamadı.");

            // DTO'dan gelen VenueId null olabilir, bu durumda henüz bir mekan atanmamıştır.
            if (createEventDto.VenueId.HasValue)
            {
                var venue = await _venueRepository.GetByIdAsync(createEventDto.VenueId.Value);
                if (venue == null) throw new InvalidOperationException("Geçersiz mekan seçimi.");
            }

            var participants = room.UserRooms.Select(ur => ur.User).ToList();

            var newEvent = new Event
            {
                VenueId = createEventDto.VenueId,
                Title = createEventDto.Title,
                Description = createEventDto.Description,
                Status = EventStatus.Planned,
                TimeSlot = room.TimeSlot,
                Category = room.Category,
                Capacity = room.Capacity,
                Participants = participants
            };

            await _eventRepository.AddAsync(newEvent);
            // İş kuralı: Etkinlik oluşturulduktan sonra oda silinebilir.
            await _roomRepository.DeleteRoom(room);

        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task<List<Event>> GetEventsByUserIdAsync(int userId)
        {
            return await _eventRepository.GetEventsByUserIdAsync(userId);
        }

        public async Task<Event> UpdateAsync(UpdateEventDto updateEventDto)
        {
            // Repository'deki düzeltilmiş GetByIdAsync metodu ilişkili tüm verileri getirecektir.
            var existingEvent = await _eventRepository.GetByIdAsync(updateEventDto.Id);
            if (existingEvent == null)
                throw new InvalidOperationException("Güncellenecek etkinlik bulunamadı.");

            // DTO'dan gelen verilerle mevcut entity'yi güncelle.
            existingEvent.Title = updateEventDto.Title;
            existingEvent.Description = updateEventDto.Description;
            existingEvent.Status = updateEventDto.Status;
            existingEvent.EventTime = updateEventDto.EventTime;
            existingEvent.Hour = updateEventDto.Hour;
            existingEvent.VenueId = updateEventDto.VenueId;

            // Repository'deki düzeltilmiş UpdateAsync metodu, bu değişiklikleri
            // veritabanına doğru bir şekilde yansıtacaktır.
            return await _eventRepository.UpdateAsync(existingEvent);
        }
    }
}
