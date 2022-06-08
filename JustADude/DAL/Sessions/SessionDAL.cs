using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace DAL.Sessions
{
    public class SessionDAL
    {
        public static async Task<Session> GetByUserId(long userId)
        {
            using (var context = new GameContext())
            {
                return await context.Sessions.FirstAsync(e => e.UserId == userId);
            }
        }
        
        public static async Task<Session> GetByHeroId(long heroId)
        {
            using (var context = new GameContext())
            {
                return await context.Sessions.FirstAsync(e => e.HeroId == heroId);
            }
        }

        public static async Task<List<SessionInfo>> GetSessionsInfo()
        {
            using (var context = new GameContext())
            {
                var result = await (from u in context.Users
                    join s in context.Sessions on u.Id equals s.UserId
                    select new SessionInfo(s.UserId, s.GameId, s.HeroId, u.Username)).ToListAsync();
                
                return result;
            }
        }
    }
}