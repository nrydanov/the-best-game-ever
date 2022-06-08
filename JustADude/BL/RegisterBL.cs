using BL.Misc;
using DAL.Users;

namespace BL
{
    public static class RegisterBL
    {
        public static bool RegisterPlayer(string username, string password)
        {
            var existing_player = UserDAL.GetByName(username);

            if (existing_player != null)
            {
                return false;
            }

            var player = new User(username, Hash.GetStringHash(password));

            UserDAL.Create(player);
            return true;
        }
    }
}