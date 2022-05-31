using System.Collections.Generic;
using System.Linq;

namespace DAL.Sessions
{
    public class SessionDAL
    {
        public static Session GetByUserId(long user_id)
        {
            using (var context = new GameContext())
            {
                var query = from s in context.Sessions
                    where s.UserId == user_id
                    select new Session(s.UserId, s.GameId, s.HeroId);

                return query.First();
            }
        }

        public static List<Session> GetSessions()
        {
            using (var context = new GameContext())
            {
                var query = from s in context.Sessions
                    select new Session(s.UserId, s.GameId, s.HeroId);
                return query.ToList();
            }
        }
    }
}