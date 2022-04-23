using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Sessions
{
    [Table("Sessions")]
    public class Session
    {
        public Session(long userId, long gameId, long heroId)
        {
            UserId = userId;
            GameId = gameId;
            HeroId = heroId;
        }

        public Session()
        {
        }

        [Column("user_id")] public long UserId { get; }


        [Column("game_id")] public long GameId { get; }

        [Column("hero_id")] public long HeroId { get; }
    }
}