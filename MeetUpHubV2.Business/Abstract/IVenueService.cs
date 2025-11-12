using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IVenueService
    {
        Task AddAsync(Venue venue);
        Task<Venue> DeleteAsync(int id);
        Task<Venue> UpdateAsync(int id, string name,RoomCategory category, string city, string location);
        Task<Venue> GetByIdAsync(int id);
        Task<Venue> GetByNameAsync(string name);
        Task<List<Venue>> GetByCity(string city);
        Task<List<Venue>> GetAllAsync();
        Task<List<Venue>> GetByCategory(RoomCategory category);

    }
}
