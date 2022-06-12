using DAL.GameObjects;

namespace BL.Dto
{
    public class GameObject
    {
        private static long _counter;
        private readonly long _id;
        
        public readonly long ObjectId;
        public readonly string ObjectType;
        public long PosX;
        public long PosY;
        public readonly long Width;
        public readonly long Height;

        public GameObject(GameObjectEnt e)
        {
            ObjectId = e.Id;
            ObjectType = e.ObjectType;
            PosX = e.PosX;
            PosY = e.PosY;

            _id = _counter;
            _counter += 1;

            switch (ObjectType)
            {
                case "hero":
                    Width = 50;
                    Height = 95;
                    break;
                default:
                    Width = 100;
                    Height = 100;
                    break;
            }
        }

        public GameObject()
        {
            
        }

        public string Username { get; set; }

        public string Id
        {
            get
            {
                if (ObjectType == "hero") return $"hero_{Username}";

                return $"obj_{_id}";
            }
        }
    }
}