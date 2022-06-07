using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BL.Dto;

namespace BL.GameLogic.Systems
{
    public static class MoveSystem
    {
        public static ConcurrentDictionary<long, HeroProperties> properties =
            new ConcurrentDictionary<long, HeroProperties>();

        private const double Dx = 3;
        private const double Dy = 10;
        private const double AccelerationLimitX = 5;
        private const double AccelerationLimitY = 5;
        private const double GravityAcceleration = 0.1;
        private const double InertiaAcceleration = 0.1;

        private static void ApplyGravity(HeroProperties props)
        {
            props.YDelta -= GravityAcceleration;
        }

        private static void ApplyInertia(HeroProperties props)
        {
            const double eps = 1e-6;
            if (props.XDelta > -eps && props.XDelta < eps) return;

            props.XDelta += -Math.Sign(props.XDelta) * InertiaAcceleration;
        }

        private static void ApplyMove(HeroProperties heroProperties, IList<int> keys)
        {
            foreach (var key in keys)
                switch (key)
                {
                    case 38: // Up
                        heroProperties.YDelta += Dy;
                        break;
                    case 40: // Down
                        heroProperties.YDelta -= Dy;
                        break;
                    case 39: // Right
                        heroProperties.XDelta += Dx;
                        break;
                    case 37: // Left
                        heroProperties.XDelta -= Dx;
                        break;
                }

            var delta_x = Math.Min(Math.Abs(heroProperties.XDelta),
                AccelerationLimitX);
            var delta_y = Math.Min(Math.Abs(heroProperties.YDelta),
                AccelerationLimitY);

            heroProperties.XDelta = Math.Sign(heroProperties.XDelta) * delta_x;
            heroProperties.YDelta = Math.Sign(heroProperties.YDelta) * delta_y;
        }

        private static void UpdateHeroState(HeroProperties heroProperties, IList<int> keys)
        {
            ApplyMove(heroProperties, keys);
            ApplyInertia(heroProperties);
            ApplyGravity(heroProperties);
        }

        public static void Update(List<GameObject> objects,
            IList<int> keys, GameObject hero)
        {
            if (!properties.ContainsKey(hero.ObjectId))
            {
                var props = new HeroProperties();
                properties.TryAdd(hero.ObjectId, props);
            }

            var hero_properties = properties[hero.ObjectId];

            UpdateHeroState(hero_properties, keys);

            CollisionSystem.Update(hero, objects);

            hero.PosX += (long)hero_properties.XDelta;
            hero.PosY += (long)hero_properties.YDelta;
        }

        public class HeroProperties
        {
            public HeroProperties()
            {
                XDelta = 0;
                YDelta = 0;
            }

            public double XDelta { get; set; }
            public double YDelta { get; set; }
        }
    }
}