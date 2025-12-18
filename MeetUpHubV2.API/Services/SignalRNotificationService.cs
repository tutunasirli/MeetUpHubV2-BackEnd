using MeetUpHubV2.API.SignalR;
using MeetUpHubV2.Business.Abstract;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetUpHubV2.API.Services
{
    public class SignalRNotificationService : IRoomNotificationService
    {
        private readonly IHubContext<RoomHub> _hubContext;

        public SignalRNotificationService(IHubContext<RoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        private string GetUserGroupName(string userId) => $"user_{userId}";
        private string GetRoomGroupName(string roomId) => $"room_{roomId}";

        // ✅ ODA DOLUNCA ÇAĞRILIR (f3.png → voting ekranı)
        public async Task SendMatchNotificationAsync(
            List<string> userIds,
            string roomId,
            object votingOptions)
        {
            // 🔥 ÖNEMLİ DÜZELTME:
            // MatchFound mesajı USER değil ROOM grubuna gider

            await _hubContext.Clients
                .Group(GetRoomGroupName(roomId))
                .SendAsync("MatchFound", new
                {
                    RoomId = roomId,
                    Options = votingOptions
                });
        }

        // ✅ OYLAMA BİTİNCE ÇAĞRILIR (f4.png → event ekranı)
        public async Task SendVotingFinishedAsync(string roomId, object eventDetails)
        {
            await _hubContext.Clients
                .Group(GetRoomGroupName(roomId))
                .SendAsync("VotingFinished", eventDetails);
        }
    }
}
