using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MeetUpHubV2.Entities; // <<< DÜZELTİLDİ: Artık 'Entities.Dtos' değil

namespace MeetUpHubV2.API.SignalR 
{
    [Authorize]
    public class RoomHub : Hub
    {
        private readonly IMemoryCache _cache;
        
        private string GetCacheKey(string roomId) => $"voting_session_{roomId}";
        private string GetRoomGroupName(string roomId) => $"room_{roomId}";
        private string GetUserGroupName(string userId) => $"user_{userId}";

        public RoomHub(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task RegisterUserConnection()
        {
            // ... (Metot içeriği aynı) ...
            string userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
            }
        }

        public async Task JoinRoomGroup(string roomId)
        {
            // ... (Metot içeriği aynı, 'VotingSession' kullanıyor) ...
            string roomGroupName = GetRoomGroupName(roomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomGroupName);

            var session = _cache.Get<VotingSession>(GetCacheKey(roomId)); // Bu satır artık doğru 'using'i bulacak
            
            if (session != null)
            {
                await Clients.Caller.SendAsync("ReceiveCurrentVotes", session);
            }
        }

        public async Task LeaveRoomGroup(string roomId)
        {
            // ... (Metot içeriği aynı) ...
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetRoomGroupName(roomId));
        }

        public async Task CastVenueVote(string roomId, int venueId)
        {
            // ... (Metot içeriği aynı, 'VotingSession' kullanıyor) ...
            string cacheKey = GetCacheKey(roomId);
            var session = await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); 
                return Task.FromResult(new VotingSession(roomId)); // Bu satır artık doğru 'using'i bulacak
            });

            int newVoteCount = session.AddVenueVote(venueId);

            await Clients.Group(GetRoomGroupName(roomId))
                         .SendAsync("ReceiveVenueVote", venueId, newVoteCount);
        }

        public async Task CastTimeVote(string roomId, string timeSlot)
        {
            // ... (Metot içeriği aynı, 'VotingSession' kullanıyor) ...
            string cacheKey = GetCacheKey(roomId);
            var session = await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return Task.FromResult(new VotingSession(roomId)); // Bu satır artık doğru 'using'i bulacak
            });

            int newVoteCount = session.AddTimeVote(timeSlot);

            await Clients.Group(GetRoomGroupName(roomId))
                         .SendAsync("ReceiveTimeVote", timeSlot, newVoteCount);
        }
    }
}