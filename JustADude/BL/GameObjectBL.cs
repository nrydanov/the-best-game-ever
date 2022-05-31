using System.Collections.Generic;
using BL.Dto;
using DAL.GameObjects;

namespace BL
{
    public static class GameObjectBL
    {
        public static List<GameObject> GetObjectsByGameId(long gameId)
        {
            var entities = GameObjectDAL.GetObjectsByGameId(gameId);
            var objects = entities.ConvertAll(e => new GameObject(e));

            return objects;
        }
    }
}