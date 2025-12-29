using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.EventDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IEventService
    {
        Task<Event> GetByIdAsync(int id);
        Task<List<Event>> GetAllAsync();
        Task AddAsync(CreateEventDto createEventDto);
        Task<Event> UpdateAsync(UpdateEventDto updateEventDto);
        Task<List<Event>> GetEventsByUserIdAsync(int userId);

        Task<bool> IsEventFinishedAsync(int eventId);


    }
}
