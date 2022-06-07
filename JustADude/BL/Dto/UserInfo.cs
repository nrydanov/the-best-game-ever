using System.Net.Sockets;
using DAL.Players;

namespace BL.Dto
{
    public class UserInfo
    {
        public UserInfo(long gameId, GameObject hero)
        {
            GameId = gameId;
            Hero = hero;

        }

        public long GameId { get; set; }
        public GameObject Hero { get; set; }
    }
}