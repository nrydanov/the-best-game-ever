﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Sessions;
using Microsoft.EntityFrameworkCore;

namespace DAL.Users
{
    public class UserDAL
    {
        public static async Task<bool> Create(User user)
        {
            using (var context = new GameContext())
            {
                
                try
                {
                    await context.AddAsync(user);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public static async Task<User> GetByName(string username)
        {
            User user;
            using (var context = new GameContext())
            {
                var query = context.Users.Where(e => e.Username == username);
                try
                {
                    user = await query.FirstAsync();
                }
                catch (InvalidOperationException)
                {
                    user = null;
                }
            }
            return user;
        }
        
        public static async Task<List<User>> GetUsers()
        {
            using (var context = new GameContext())
            {
                return await context.Users.ToListAsync();
            }
        }
    }
}