using System.Collections.Generic;
using System.Threading.Tasks;

// Namespace'in doğru olduğundan emin ol
namespace MeetUpHubV2.Business.Abstract
{
    public interface IRoomNotificationService
    {
        // <<< HATA (CS1061) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        Task SendMatchNotificationAsync(List<string> userIds, string roomId, object votingOptions);
        
        // <<< HATA (CS1061) BUNUN EKSİK OLDUĞUNU SÖYLÜYOR
        Task SendVotingFinishedAsync(string roomId, object eventDetails);

        // (Varsa, 'NotifyUserJoined' gibi diğer metotların da burada olabilir)
    }
}