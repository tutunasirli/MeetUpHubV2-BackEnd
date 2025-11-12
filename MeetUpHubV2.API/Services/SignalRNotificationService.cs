using MeetUpHubV2.API.SignalR; // RoomHub'ı tanımak için
using MeetUpHubV2.Business.Abstract; // IRoomNotificationService'i tanımak için
using Microsoft.AspNetCore.SignalR; // IHubContext'i tanımak için
using System.Collections.Generic; // List<string> için
using System.Threading.Tasks;

// Namespace'in doğru olduğundan emin ol
namespace MeetUpHubV2.API.Services
{
    // IRoomNotificationService kontratını uyguluyoruz
    public class SignalRNotificationService : IRoomNotificationService
    {
        // Hub'ın dışından (servis katmanından) Hub'a erişmek için
        // 'IHubContext' kullanılır.
        private readonly IHubContext<RoomHub> _hubContext;

        public SignalRNotificationService(IHubContext<RoomHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // RoomHub'da tanımladığımız yardımcı metotlar
        private string GetUserGroupName(string userId) => $"user_{userId}";
        private string GetRoomGroupName(string roomId) => $"room_{roomId}";


        // --- EKSİK METOT 1 (CS0535 HATASINI ÇÖZER) ---
        // RoomManager, bir eşleşme bulduğunda (f.png) bu metodu çağıracak
        public async Task SendMatchNotificationAsync(List<string> userIds, string roomId, object votingOptions)
        {
            // Eşleşen herkese (Isa ve Tutu)
            // "MatchFound" (Eşleşme Bulundu) mesajını gönderir.
            // Frontend bu mesajı yakalayıp f3.png ekranına geçmelidir.

            foreach (var userId in userIds)
            {
                await _hubContext.Clients
                    .Group(GetUserGroupName(userId)) // Örn: "user_Isa" grubuna
                    .SendAsync("MatchFound", new { RoomId = roomId, Options = votingOptions });
            }
        }

        // --- EKSİK METOT 2 (CS0535 HATASINI ÇÖZER) ---
        // 30 saniye dolup oylama bittiğinde (f4.png) bu metot çağrılacak
        public async Task SendVotingFinishedAsync(string roomId, object eventDetails)
        {
            // Oylama odasındaki (f3.png) herkese
            // "VotingFinished" (Oylama Bitti) mesajını gönderir.
            
            await _hubContext.Clients
                .Group(GetRoomGroupName(roomId)) // Örn: "room_oda123" grubuna
                .SendAsync("VotingFinished", eventDetails);
        }

        // (Eğer NotifyUserJoined gibi eski metotların varsa
        // ve IRoomNotificationService'de de tanımlıysalar burada kalabilirler)
    }
}