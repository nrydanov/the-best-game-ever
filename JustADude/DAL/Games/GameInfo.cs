using System;
using System.Collections.Generic;

namespace DAL.Games
{
    public class GameInfo
    {
        public DateTime Created;
        public string Host;
        public long Id;
        public IList<string> Joined;

        public GameInfo(long id, string hostName, DateTime created)
        {
            Id = id;
            Host = hostName;
            Created = created;
        }
    }
}