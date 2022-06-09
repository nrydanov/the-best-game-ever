using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using DAL.GameObjects;
using DAL.Sessions;

namespace DAL.Games
{
    public static class GameDAL
    {
        public static async Task<bool> Create(Game game)
        {
            await using (var context = new GameContext())
            {
                try
                {
                    await context.AddAsync(game);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public static async Task<Game> GetByHostName(string name)
        {
            long id;
            await using (var context = new GameContext())
            {
                var query = context.Users.Where(e => e.Username == name);
                try
                {
                    id = query.FirstAsync().Id;
                }
                catch (InvalidOperationException)
                {
                    id = -1;
                }
            }

            return await GetByHostId(id);
        }

        public static async Task<Game> GetByHostId(long id)
        {
            Game game;
            await using (var context = new GameContext())
            {
                var query = context.Games.Where(e => e.HostId == id);
                try
                {
                    game = await query.FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    game = null;
                }
            }

            return game;
        }

        public static async Task<IList<GameInfo>> GetGames()
        {
            await using (var context = new GameContext())
            {
                var result = await (from g in context.Games
                    join p in context.Users on g.HostId equals p.Id
                    join u in context.Sessions on g.Id equals u.GameId into temp
                    from j in temp.DefaultIfEmpty()
                    select new GameInfo(g.Id, p.Username, g.Created)).Distinct().ToListAsync();
                
                foreach (var r in result)
                {
                    var joined = await (from s in context.Sessions
                        where s.GameId == r.Id
                        join p in context.Users on s.UserId equals p.Id
                        select new string(p.Username)).Distinct().ToListAsync();

                    r.Joined = joined;
                }

                return result;
            }
        }

        public static async Task<long> JoinUser(long userId, long gameId)
        {
            await using (var context = new GameContext())
            {
                var obj = new GameObjectEnt(gameId, "hero", 100, 100);
                await context.GameObjectEnt.AddAsync(obj);
                await context.SaveChangesAsync();

                var session = new Session(userId, gameId, obj.Id);

                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();
                return obj.Id;
            }
        }
        
        public static void DropUser(long userId)
        {
            using (var context = new GameContext())
            {
                var session = context.Sessions.First(e => e.UserId == userId);
                var hero = 
                    context.GameObjectEnt.First(e => e.Id == session.HeroId);

                context.Remove(session);
                context.SaveChanges();

                context.Remove(hero);
                context.SaveChanges();
            }
        }
    }
}