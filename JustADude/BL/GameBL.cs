using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using DAL.Sessions;
using DAL.Users;
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
        private static ConcurrentDictionary<long, ConcurrentDictionary<string, bool>> events;

        public const long UpdateGameStateRate = 25;
        
        static GameBL()
        {
            connectedUsers = RestoreConnectedUsers().Result;
            gameObjects = new ConcurrentDictionary<long, List<GameObject>>();
            userIds = RestoreUserIds().Result;
            events = new ConcurrentDictionary<long, ConcurrentDictionary<string, bool>>();
            
            var timer = new Timer(UpdateGameStateRate);
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

        private static async Task<ConcurrentDictionary<long, UserInfo>> RestoreConnectedUsers()
        {
            var sessions = await SessionDAL.GetSessionsInfo();
            var dict =
                    new ConcurrentDictionary<long, UserInfo>();

            foreach (var s in sessions)
            {
                var e = GameObjectDAL.GetObjectById(s.HeroId);

                dict.TryAdd(s.UserId, new UserInfo(s.GameId, null));
            }
            
            return dict;
        }

        private static async Task<ConcurrentDictionary<string, long>> RestoreUserIds()
        {
            var users = await UserDAL.GetUsers();

            var dict = new ConcurrentDictionary<string, long>();

            for (int i = 0; i < users.Count; ++i)
            {
                dict.TryAdd(users[i].Username, users[i].Id);
            }

            return dict;
        }

        public static async Task<bool> Create(String hostName)
        {
            long id;
            try
            {
                id = UserDAL.GetByName(hostName).Id;
            }
            catch(NullReferenceException)
            {
                return false;
            }
            
            var game = new Game(id, DateTime.Now);
            return await GameDAL.Create(game);
        }

        private static async Task<bool> LoadObjectsFromDB(long gameId)
        {
            var objs = await GameObjectBL.GetObjectsByGameId(gameId);
            var heroes = objs.FindAll(e => e.ObjectType == "hero");

            for (int i = 0; i < heroes.Count; ++i)
            {
                var session = await SessionDAL.GetByHeroId(heroes[i].ObjectId);
                var user_id = session.UserId;
                connectedUsers[user_id].Hero = heroes[i];
            }

            return gameObjects.TryAdd(gameId, objs);
        }

        public static async Task<bool> Join(string username, long gameId)
        {

            var user = await UserDAL.GetByName(username);
            var user_id = user.Id;

            if (connectedUsers.ContainsKey(user_id))
            {
                return false;
            }

            if (!gameObjects.ContainsKey(gameId))
            {
                await LoadObjectsFromDB(gameId);
            }

            var hero_id = await GameDAL.JoinUser(user_id, gameId);

            var entity = await GameObjectDAL.GetObjectById(hero_id);
            var hero = new GameObject(entity);
            
            hero.Username = username;

            gameObjects[gameId].Add(hero);
            connectedUsers.TryAdd(user.Id, new UserInfo(gameId, hero));
            userIds.TryAdd(username, user_id);
            events.TryAdd(user_id, new ConcurrentDictionary<string, bool>());

            return true;
        }

        public static async Task<bool> Leave(string user)
        {
            var player = await UserDAL.GetByName(user);

            if (!connectedUsers.ContainsKey(player.Id))
            {
                return false;
            }

            connectedUsers.TryRemove(player.Id, out _);

            return true;
        }

        public static async Task<IList<GameInfo>> GetGames()
        {
            var games = await GameDAL.GetGames();

            return games;
        }

        public static async Task<List<GameObject>> GetObjects(string user)
        {
            if (!userIds.ContainsKey(user))
            {
                var player = await UserDAL.GetByName(user);
                userIds.TryAdd(user, player.Id);
            }
            
            var user_id = userIds[user];

            if (!connectedUsers.ContainsKey(user_id))
            {
                // TODO: Maybe more proper reaction in such case
                return new List<GameObject>();
            }
            
            var info = connectedUsers[user_id];
            var game_id = info.GameId;
            
            if (!gameObjects.ContainsKey(game_id))
            {
                await LoadObjectsFromDB(game_id);
            }
            
            return gameObjects[game_id];
        }
        
        private static bool convertBitToFlag(string mask, int index)
        {
            return mask[index] != '0';
        }

        public static void Update(string user, string mask)
        {
            var keys = new Dictionary<string, bool>();
            
            keys["ArrowUp"] = convertBitToFlag(mask, 0);
            keys["ArrowDown"] = convertBitToFlag(mask, 1);
            keys["ArrowRight"] = convertBitToFlag(mask, 2);
            keys["ArrowLeft"] = convertBitToFlag(mask, 3);

            var user_id = userIds[user];

            if (!events.ContainsKey(user_id))
            {
                events.TryAdd(user_id, new ConcurrentDictionary<string, bool>());
            }

            foreach (var key_event in keys)
            {
                events[user_id].AddOrUpdate(key_event.Key, 
                    key_event.Value, (key, value) => key_event.Value);
            }
        }
    }
}
