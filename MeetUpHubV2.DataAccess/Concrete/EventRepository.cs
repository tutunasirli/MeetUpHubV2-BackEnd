using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.DataAccess.Concrete
{
    public class EventRepository : IEventRepository
    {
        // 1. DbContext için özel bir alan (field) tanımladık
        private readonly MeetUpHubV2DbContext _context;

        // 2. DbContext'i parametre olarak alan bir CONSTRUCTOR (yapıcı metot) ekledik
        public EventRepository(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        // 3. Aşağıdaki TÜM METOTLARI 'using' yerine '_context' kullanacak şekilde güncelledik

        public async Task AddAsync(Event eventEntity)
        {
            if (eventEntity.Participants != null)
            {
                foreach (var participant in eventEntity.Participants)
                {
                    _context.Users.Attach(participant);
                }
            }
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Participants)
                .ToListAsync();
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Event>> GetEventsByUserIdAsync(int userId)
        {
            return await _context.Events
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p => p.Id == userId))
                .ToListAsync();
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }
    }
}