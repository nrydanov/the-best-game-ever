using System;
using System.Linq;

namespace DAL.Players
{
    public class PlayerDAL
    {
        public static bool Create(Player player)
        {
            using (var context = new GameContext())
            {
                context.Add(player);
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

        public static Player GetByName(string username)
        {
            Player player;
            using (var context = new GameContext())
            {
                var query = context.Players.Where(e => e.Username == username);
                try
                {
                    player = query.First();
                }
                catch (InvalidOperationException)
                {
                    player = null;
                }
            }

            return player;
        }
    }
}