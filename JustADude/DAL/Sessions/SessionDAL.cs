using System.Collections.Generic;
using System.Linq;
using DAL.Users;

namespace DAL.Sessions
{
    public class SessionDAL
    {
        public static Session GetByUserId(long userId)
        {
            using (var context = new GameContext())
            {
                return context.Sessions.First(e => e.UserId == userId);
            }
        }
        
        public static Session GetByHeroId(long heroId)
        {
            using (var context = new GameContext())
            {
                return context.Sessions.First(e => e.HeroId == heroId);
            }
        }

        public static List<SessionInfo> GetSessionsInfo()
        {
            using (var context = new GameContext())
            {
                var query = from u in context.Users
                    join s in context.Sessions on u.Id equals s.UserId
                    select new SessionInfo(s.UserId, s.GameId, s.HeroId, u.Username);
                
                return query.ToList();
            }
        }
    }
}