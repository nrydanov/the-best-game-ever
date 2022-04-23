using BL.Misc;
using DAL.Players;

namespace BL
{
    public static class RegisterBL
    {
        public static bool RegisterPlayer(string username, string password)
        {
            var existing_player = PlayerDAL.GetByName(username);

            if (existing_player != null)
            {
                return false;
            }

            var player = new Player(username, Hash.GetStringHash(password));

            PlayerDAL.Create(player);
            return true;
        }
    }
}