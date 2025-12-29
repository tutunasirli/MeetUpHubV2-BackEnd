using MeetUpHubV2.Entities;

namespace MeetUpHubV2.Business.Abstract
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<bool> UpdateAboutAsync(int userId, string? about);

    }
}
