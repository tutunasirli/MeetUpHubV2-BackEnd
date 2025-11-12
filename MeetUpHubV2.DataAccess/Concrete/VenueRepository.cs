using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // List için eklendi
using System.Linq; // Where, ToListAsync için eklendi
using System.Threading.Tasks; // Task için eklendi

namespace MeetUpHubV2.DataAccess.Concrete
{
    public class VenueRepository : IVenueRepository
    {
        // 1. DbContext için özel bir alan (field) tanımladık
        private readonly MeetUpHubV2DbContext _context;

        // 2. DbContext'i parametre olarak alan bir CONSTRUCTOR (yapıcı metot) ekledik
        public VenueRepository(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        // 3. Aşağıdaki TÜM METOTLARI 'using' yerine '_context' kullanacak şekilde güncelledik

        public async Task AddAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
        }

        public async Task<Venue> DeleteAsync(int id)
        {
            var deleted = await _context.Venues.FirstOrDefaultAsync(x => x.Id == id);
            if (deleted != null)
            {
                _context.Venues.Remove(deleted);
                await _context.SaveChangesAsync();
            }
            return deleted; // Silinen nesneyi (veya null) döndür
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            // Hata düzeltmesi: ToList() yerine ToListAsync() kullanıldı.
            return await _context.Venues.ToListAsync();
        }

        public async Task<List<Venue>> GetByCategory(RoomCategory category)
        {
            return await _context.Venues
                .Where(x => x.Category == category)
                .ToListAsync();
        }

        public async Task<List<Venue>> GetByCity(string city)
        {
            return await _context.Venues
                .Where(x => x.City.ToLower() == city.ToLower())
                .ToListAsync(); // Liste döndür
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            // FirstAsync yerine FirstOrDefaultAsync kullanmak daha güvenlidir.
            return await _context.Venues.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Venue> GetByNameAsync(string name)
        {
            return await _context.Venues.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<Venue> UpdateAsync(int id, string name, RoomCategory category, string city, string location)
        {
            var existingVenue = await _context.Venues.FindAsync(id);

            if (existingVenue != null)
            {
                existingVenue.Name = name;
                existingVenue.Category = category;
                existingVenue.City = city;
                existingVenue.Location = location;

                _context.Venues.Update(existingVenue);
                await _context.SaveChangesAsync();
            }

            return existingVenue;
        }
    }
}