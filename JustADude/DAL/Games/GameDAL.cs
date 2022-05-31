using System;
using System.Collections.Generic;
using System.Linq;
using DAL.GameObjects;
using DAL.Sessions;

namespace DAL.Games
{
    public class GameDAL
    {
        public static bool Create(Game game)
        {
            using (var context = new GameContext())
            {
                context.Add(game);
                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public static Game GetByHostName(string name)
        {
            long id;
            using (var context = new GameContext())
            {
                var query = context.Players.Where(e => e.Username == name);
                try
                {
                    id = query.First().Id;
                }
                catch (InvalidOperationException)
                {
                    id = -1;
                }
            }

            return GetByHostId(id);
        }

        public static Game GetByHostId(long id)
        {
            Game game;
            using (var context = new GameContext())
            {
                var query = context.Games.Where(e => e.HostId == id);
                try
                {
                    game = query.First();
                }
                catch (InvalidOperationException)
                {
                    game = null;
                }
            }

            return game;
        }

        public static IList<GameInfo> GetGames()
        {
            using (var context = new GameContext())
            {
                var query = from g in context.Games
                    join p in context.Players on g.HostId equals p.Id
                    join u in context.Sessions on g.Id equals u.GameId into temp
                    from j in temp.DefaultIfEmpty()
                    select new GameInfo(g.Id, p.Username, g.Created);


                IList<GameInfo> result = query.Distinct().ToList();
                foreach (var r in result)
                {
                    var joined = (from s in context.Sessions
                        where s.GameId == r.Id
                        join p in context.Players on s.UserId equals p.Id
                        select new string(p.Username)).Distinct().ToList();

                    r.Joined = joined;
                }

                return result;
            }
        }

        public static long JoinUser(long user_id, long game_id)
        {
            using (var context = new GameContext())
            {
                var obj = new GameObjectEnt(game_id, "hero", 100, 100);
                context.Add(obj);
                context.SaveChanges();

                var session = new Session(user_id, game_id, obj.Id);

                context.Add(session);
                context.SaveChanges();
                return obj.Id;
            }
        }
    }
}