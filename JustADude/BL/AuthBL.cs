using System;
using BL.Misc;
using DAL.Players;

namespace BL
{
    public static class AuthBL
    {
        public static bool Authorize(string username, string password)
        {
            try
            {
                var player = PlayerDAL.GetByName(username);
                return player.Password.Equals(Hash.GetStringHash(password));
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}