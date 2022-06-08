using System;
using System.Threading.Tasks;
using BL.Misc;
using DAL.Users;

namespace BL
{
    public static class AuthBL
    {
        public static async Task<bool> Authorize(string username, string password)
        {
            try
            {
                var player = await UserDAL.GetByName(username);
                return player.Password.Equals(Hash.GetStringHash(password));
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}