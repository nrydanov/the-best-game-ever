using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Games
{
    [Table("Games")]
    public class Game
    {
        public Game(long host_id, DateTime created)
        {
            Created = created;
            HostId = host_id;
        }

        public Game()
        {
        }

        [Column("id")] public long Id { get; set; }

        [Column("created")] public DateTime Created { get; set; }

        [Column("host_id")] public long HostId { get; set; }
    }
}