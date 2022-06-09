using System.Threading.Tasks;

using BL.Misc;
using DAL.Users;

namespace BL
{
    public static class RegisterBL
    {
        public static async Task<bool> RegisterPlayer(string username, string password)
        {
            var existing_player = await UserDAL.GetByName(username);

            if (existing_player != null)
            {
                return false;
            }

            var player = new User(username, Hash.GetStringHash(password));

            await UserDAL.Create(player);
            return true;
        }
    }
}