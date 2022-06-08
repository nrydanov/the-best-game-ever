using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Dto;
using DAL.GameObjects;

namespace BL
{
    public static class GameObjectBL
    {
        public static async Task<List<GameObject>> GetObjectsByGameId(long gameId)
        {
            var entities = await GameObjectDAL.GetObjectsByGameId(gameId);
            var objects = entities.ConvertAll(e => new GameObject(e));

            return objects;
        }
    }
}