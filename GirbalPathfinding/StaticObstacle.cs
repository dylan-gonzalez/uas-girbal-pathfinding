using System;
using System.Collections.Generic;
using System.Text;

namespace GirbalPathfinding
{
    public struct StaticObstacle : IObstacleable
    {
        public int x;
        public int y;
        public int z;

        public StaticObstacle(int xn, int yn, int zn)
        {
            x = xn;
            y = yn;
            z = zn;
        }

        public bool Equals(StaticObstacle obj)
        {
            return obj.x == x && obj.y == y && obj.z == z; 
        }
    }
}
