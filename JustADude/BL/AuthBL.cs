using System;
using BL.Misc;
using DAL.Users;

namespace BL
{
    public static class AuthBL
    {
        public static bool Authorize(string username, string password)
        {
            try
            {
                var player = UserDAL.GetByName(username);
                return player.Password.Equals(Hash.GetStringHash(password));
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}