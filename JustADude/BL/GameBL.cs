using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using DAL.Sessions;
using DAL.Players;
using DAL.Games;
using DAL.GameObjects;
using BL.Dto;
using BL.GameLogic.Systems;

namespace BL
{
    public static class GameBL
    {
        private static ConcurrentDictionary<long, UserInfo> connectedUsers;
        private static ConcurrentDictionary<long, List<GameObject>> gameObjects;
        private static ConcurrentDictionary<string, long> userIds;
        private static ConcurrentDictionary<long, ConcurrentStack<string>> events;
        static GameBL()
        {
            connectedUsers = RestoreConnectedUsers();
            gameObjects = new ConcurrentDictionary<long, List<GameObject>>();
            userIds = new ConcurrentDictionary<string, long>();
            events = new ConcurrentDictionary<long, ConcurrentStack<string>>();

            var timer = new Timer(10);
            timer.Elapsed += UpdateGameState;
            timer.Start();
            
        }

        private static void UpdateGameState(object sender, EventArgs args)
        {
            foreach (var e in events)
            {
                var user_id = e.Key;
                var actions = e.Value;

                var info = connectedUsers[user_id];
                if (!gameObjects.ContainsKey(info.GameId))
                {
                    // TODO: Maybe add more proper handle in such case
                    return;
                }
                var objects = gameObjects[info.GameId];
                var hero = info.Hero;

                MoveSystem.Update(objects, actions, hero);
            }
        }

        private static ConcurrentDictionary<long, UserInfo> RestoreConnectedUsers()
        {
            var sessions = SessionDAL.GetSessions();
            var dict =
                    new ConcurrentDictionary<long, UserInfo>();

            foreach (var s in sessions)
            {
                var e = GameObjectDAL.GetObjectById(s.HeroId);
                
                dict.TryAdd(s.UserId, new UserInfo(s.GameId, new GameObject(e)));
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
            var objs = GameObjectBL.GetObjectsByGameId(gameId);
            gameObjects.TryAdd(gameId, objs);
        }

        public static bool Join(string user, long gameId)
        {

            var player = PlayerDAL.GetByName(user);
            var user_id = player.Id;

            if (connectedUsers.ContainsKey(user_id))
            {
                return false;
            }

            if (!gameObjects.ContainsKey(gameId))
            {
                LoadObjectsFromDB(gameId);
            }

            var hero_id = GameDAL.JoinUser(user_id, gameId);
            var entity = GameObjectDAL.GetObjectById(hero_id);
            var hero = new GameObject(entity);
            
            hero.Username = user;

            gameObjects[gameId].Add(hero);
            connectedUsers.TryAdd(player.Id, new UserInfo(gameId, hero));
            userIds.TryAdd(user, user_id);
            events.TryAdd(user_id, new ConcurrentStack<string>());

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

        public static List<GameObject> GetObjects(string user)
        {
            if (!userIds.ContainsKey(user))
            {
                var player = PlayerDAL.GetByName(user);
                userIds.TryAdd(user, player.Id);
                events[player.Id] = new ConcurrentStack<string>();
            }
            
            var user_id = userIds[user];
            var info = connectedUsers[user_id];
            var game_id = info.GameId;
            
            if (!gameObjects.ContainsKey(game_id))
            {
                LoadObjectsFromDB(game_id);
            }
            
            return gameObjects[game_id];
        }

        public static void Update(string user, IList<string> keys)
        {
            if (!userIds.ContainsKey(user))
            {
                var player = PlayerDAL.GetByName(user);
                userIds.TryAdd(user, player.Id);
            }
            
            var user_id = userIds[user];

            if (!events.ContainsKey(user_id))
            {
                events.TryAdd(user_id, new ConcurrentStack<string>());
            }

            for (int i = 0; i < keys.Count; ++i)
            {
                events[user_id].Push(keys[i]);
            }
        }
    }

    
}
