using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Sessions;

namespace DAL.Users
{
    public class UserDAL
    {
        public static bool Create(User user)
        {
            using (var context = new GameContext())
            {
                context.Add(user);
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

        public static User GetByName(string username)
        {
            User user;
            using (var context = new GameContext())
            {
                var query = context.Users.Where(e => e.Username == username);
                try
                {
                    user = query.First();
                }
                catch (InvalidOperationException)
                {
                    user = null;
                }
            }

            return user;
        }
        
        public static List<User> GetUsers()
        {
            using (var context = new GameContext())
            {
                return context.Users.ToList();
            }
        }
    }
}