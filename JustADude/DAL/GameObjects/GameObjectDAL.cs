using System.Collections.Generic;
using System.Linq;

namespace DAL.GameObjects
{
    public static class GameObjectDAL
    {
        public static List<GameObjectEnt> GetObjectsByGameId(long game_id)
        {
            using (var context = new GameContext())
            {
                var query = from o in context.GameObjectEnt
                    where o.GameId == game_id
                    select new GameObjectEnt(o.GameId,
                        o.ObjectType,
                        o.PosX,
                        o.PosY, o.Id);
                return query.ToList();
            }
        }

        public static GameObjectEnt GetObjectById(long id)
        {
            using (var context = new GameContext())
            {
                var query = from o in context.GameObjectEnt
                    where o.Id == id
                    select new GameObjectEnt(o.GameId,
                        o.ObjectType,
                        o.PosX,
                        o.PosY,
                        o.Id);
                return query.First();
            }
        }
    }
}