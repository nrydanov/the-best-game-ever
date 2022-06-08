using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.GameObjects
{
    public static class GameObjectDAL
    {
        public static async Task<List<GameObjectEnt>> GetObjectsByGameId(long gameId)
        {
            await using (var context = new GameContext())
            {
                var result = 
                    await context.GameObjectEnt.Where(e => e.GameId == gameId).ToListAsync();
                return result;
            }
        }

        public static async Task<GameObjectEnt> GetObjectById(long id)
        {
            await using (var context = new GameContext())
            {
                var result = await context.GameObjectEnt.Where(e => e.Id == id).FirstAsync();
                return result;
            }
        }
    }
}