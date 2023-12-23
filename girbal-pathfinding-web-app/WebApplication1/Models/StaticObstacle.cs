using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Models
{
    public struct StaticObstacle
    {
        public int x;
        public int y;

        public StaticObstacle(int xn, int yn)
        {
            x = xn;
            y = yn;
        }

        public bool Equals(StaticObstacle obj)
        {
            return obj.x == x && obj.y == y;
        }
    }
}
