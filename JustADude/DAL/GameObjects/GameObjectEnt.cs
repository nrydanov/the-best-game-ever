using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.GameObjects
{
    [Table("GameObjects")]
    public class GameObjectEnt
    {
        public GameObjectEnt(long game_id, string type, long pos_x, long pos_y, long id = 0)
        {
            Id = id;
            GameId = game_id;
            ObjectType = type;
            PosX = pos_x;
            PosY = pos_y;
        }

        public GameObjectEnt()
        {
        }

        [Column("id")] public long Id { get; set; }

        [Column("game_id")] public long GameId { get; set; }

        [Column("object_type")] public string ObjectType { get; set; }

        [Column("pos_x")] public long PosX { get; set; }

        [Column("pos_y")] public long PosY { get; set; }
    }
}