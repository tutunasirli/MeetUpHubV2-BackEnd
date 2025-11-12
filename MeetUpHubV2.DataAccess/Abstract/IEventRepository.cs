using MeetUpHubV2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Abstract
{
    public interface IEventRepository
    {
        Task<Event> GetByIdAsync(int id);
        Task<List<Event>> GetAllAsync();
        Task AddAsync(Event eventEntity);
        Task<Event> UpdateAsync(Event eventEntity);

        Task<List<Event>> GetEventsByUserIdAsync(int userId); //Kullanıcıların içinde bulunduğu etkinlikleri getirmek için.

    }
}
