using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.DataAccess;
using MeetUpHubV2.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetUpHubV2.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly MeetUpHubV2DbContext _context;

        public UserManager(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // 🔹 EKLENEN METOT (HATAYI ÇÖZER)
        public async Task<bool> UpdateAboutAsync(int userId, string? about)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            user.About = about;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
