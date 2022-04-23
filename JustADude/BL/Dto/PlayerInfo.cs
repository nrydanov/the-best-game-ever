namespace BL.Dto
{
    public class PlayerInfo
    {
        public PlayerInfo(long game_id, long hero_id)
        {
            GameId = game_id;
            HeroId = hero_id;
        }

        public long GameId { get; set; }
        public long HeroId { get; set; }
    }
}