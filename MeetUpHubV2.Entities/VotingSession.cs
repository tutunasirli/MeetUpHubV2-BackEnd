using System.Collections.Concurrent;

// <<< DÜZELTİLDİ: Namespace 'Dtos' alt klasöründe değil
namespace MeetUpHubV2.Entities 
{
    public class VotingSession
    {
        public string RoomId { get; set; }

        // Hata (CS1061) bunların eksik olduğunu söylüyordu
        public ConcurrentDictionary<int, int> VenueVotes { get; private set; }

        // Hata (CS1061) bunların eksik olduğunu söylüyordu
        public ConcurrentDictionary<string, int> TimeVotes { get; private set; }

        public VotingSession(string roomId)
        {
            RoomId = roomId;
            VenueVotes = new ConcurrentDictionary<int, int>();
            TimeVotes = new ConcurrentDictionary<string, int>();
        }

        public int AddVenueVote(int venueId)
        {
            return VenueVotes.AddOrUpdate(venueId, 1, (key, oldCount) => oldCount + 1);
        }

        public int AddTimeVote(string timeSlot)
        {
            return TimeVotes.AddOrUpdate(timeSlot, 1, (key, oldCount) => oldCount + 1);
        }
    }
}