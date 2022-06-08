using System;
using System.Collections.Generic;
using BL.Dto;

namespace BL.GameLogic.Systems
{
    public static class CollisionSystem
    {

        private static double CalcArea(double x1, double dx1, double y1, double dy1,
            double x2, double dx2, double y2, double dy2)
        {
            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
                (dx1, dx2) = (dx2, dx1);
                (dy1, dy2) = (dy2, dy1);
            }

            var dist_x = Math.Max(x1 + dx1 - x2, 0);

            if (y1 > y2) return dist_x * Math.Max(y2 + dy2 - y1, 0);

            return dist_x * Math.Max(y1 + dy1 - y2, 0);
        }

        public static void Update(GameObject hero, List<GameObject> objects)
        {
            foreach (var obj in objects)
                if (obj.ObjectType == "stone")
                {
                    var x1 = hero.PosX;
                    var x2 = obj.PosX;
                    var y1 = hero.PosY;
                    var y2 = obj.PosY;
                    var dx1 = hero.Width;
                    var dx2 = obj.Width;
                    var dy1 = hero.Height;
                    var dy2 = obj.Height;

                    var props = MoveSystem.properties[hero.ObjectId];
                    var area_y = CalcArea(x1, dx1, y1 + props.YDelta, dy1, x2, dx2, y2, dy2);
                    var area_x = CalcArea(x1 + props.XDelta, dx1, y1, dy1, x2, dx2, y2, dy2);

                    if (area_x > 0) props.XDelta = 0;

                    if (area_y > 0)
                    {
                        if (props.YDelta < 0)
                        {
                            props.CanJump = true;    
                        }
                        props.YDelta = 0;
                    }
                }
        }
    }
}