using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Sessions
{
    public class SessionInfo
    {
        public SessionInfo(long userId, long gameId, long heroId, string username)
        {
            UserId = userId;
            GameId = gameId;
            HeroId = heroId;
            Username = username;
        }

        public SessionInfo()
        {
            
        }

        public long UserId { get; }
        
        public string Username { get; }
        public long GameId { get; }

        public long HeroId { get; }

        
    }
}