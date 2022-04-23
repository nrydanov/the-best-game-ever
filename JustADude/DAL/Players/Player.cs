using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Players
{
    [Table("Players")]
    public class Player
    {
        public Player()
        {
        }

        public Player(string username, string password, long score = 0)
        {
            Username = username;
            Password = password;
            Score = score;
        }

        [Column("id")] public long Id { get; set; }

        [Column("username")] public string Username { get; set; }

        [Column("password")] public string Password { get; set; }

        [Column("score")] public long Score { get; set; }
    }
}