using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;


namespace MeetUpHubV2.Business.Concrete
{
    public class VenueManager : IVenueService
    {
        private readonly IVenueRepository _repository;

        public VenueManager(IVenueRepository repository)
        {
            _repository = repository;
        }
        public async Task AddAsync(Venue venue)
        {
            await _repository.AddAsync(venue);
        }

        public async Task<Venue> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<Venue>> GetByCategory(RoomCategory category)
        {
            if (category != null)
            {
                return await _repository.GetByCategory(category);
            }
            throw new Exception("Girilen kategori numarası hatalı.");
        }

        public async Task<List<Venue>> GetByCity(string city)
        {
            var venue = await _repository.GetByCity(city);
            if (venue == null)
            {
                throw new Exception("Girilen şehir de mekan bulunamadı.");
            }
            return venue;
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            var venue = await _repository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new Exception("Girilen Id de mekan bulunamadı.");
            }
            return venue;
        }

        public async Task<Venue> GetByNameAsync(string name)
        {
            var venue = await _repository.GetByNameAsync(name);
            if (venue == null)
            {
                throw new Exception("Girilen isim de mekan bulunamadı.");
            }
            return venue;
        }

        public async Task<Venue> UpdateAsync(int id, string name, RoomCategory category,string city, string location)
        {
            var venue = await _repository.GetByIdAsync(id);
            if (venue == null)
            {
                throw new Exception("Girilen Id de mekan bulunamadı.");
            }
            await _repository.UpdateAsync(id, name, category,city, location);
            return venue;
        }
    }
}
