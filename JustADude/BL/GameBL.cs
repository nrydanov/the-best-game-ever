using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DAL.Sessions;
using DAL.Players;
using DAL.Games;
using DAL.GameObjects;
using BL.GameLogic.Systems;
using BL.Dto;

namespace BL
{
    public static class GameBL
    {
        static ConcurrentDictionary<long, PlayerInfo> connectedUsers =
                RestoreConnectedUsers();

        static ConcurrentDictionary<long, List<GameObject>> gameObjects =
                new ConcurrentDictionary<long, List<GameObject>>();


        private static ConcurrentDictionary<long, PlayerInfo> RestoreConnectedUsers()
        {
            var sessions = SessionDAL.GetSessions();
            var dict =
                    new ConcurrentDictionary<long, PlayerInfo>();

            foreach (var s in sessions)
            {
                dict.TryAdd(s.UserId, new PlayerInfo(s.GameId, s.HeroId));
            }
            
            return dict;
        } 
        
        public static bool Create(String hostName)
        {
            long id;
            try
            {
                id = PlayerDAL.GetByName(hostName).Id;
            }
            catch(NullReferenceException)
            {
                return false;
            }
            
            var game = new Game(id, DateTime.Now);
            return GameDAL.Create(game);
        }

        private static void LoadObjectsFromDB(long gameId)
        {
            Console.WriteLine("Loading objects from DB");
            var objs = GameObjectBL.GetObjectsByGameId(gameId);
            gameObjects.TryAdd(gameId, objs);
        }

        public static bool Join(string user, long gameId)
        {

            var player = PlayerDAL.GetByName(user);

            if (connectedUsers.ContainsKey(player.Id))
            {
                return false;
            }

            if (!gameObjects.ContainsKey(gameId))
            {
                LoadObjectsFromDB(gameId);
            }

            var hero_id = GameDAL.JoinUser(player.Id, gameId);

            var entity = GameObjectDAL.GetObjectById(hero_id);

            var hero = new GameObject(entity);
            hero.Username = user;

            gameObjects[gameId].Add(hero);
            connectedUsers.TryAdd(player.Id, new PlayerInfo(gameId, hero_id));

            return true;
        }

        public static bool Leave(String user)
        {
            var player = PlayerDAL.GetByName(user);

            if (!connectedUsers.ContainsKey(player.Id))
            {
                return false;
            }

            connectedUsers.TryRemove(player.Id, out _);

            return true;
        }

        public static IList<GameInfo> GetGames()
        {
            var games = GameDAL.GetGames();

            return games;
        }

        public static List<GameObject> Update(String user, IList<int> keys)
        {
            var player = PlayerDAL.GetByName(user);

            if (!connectedUsers.ContainsKey(player.Id))
            {
                Console.WriteLine("Attempt to update a game for not connected user");

                return new List <GameObject>();
            }

            var info = connectedUsers[player.Id];
            var game_id = info.GameId;

            if (!gameObjects.ContainsKey(game_id))
            {
                LoadObjectsFromDB(game_id);
            }

            var objects = gameObjects[game_id];
            var hero = objects.Find(e => e.ObjectId == info.HeroId);

            MoveSystem.Update(objects, keys, hero);
            return objects;
        }
    }

    
}
