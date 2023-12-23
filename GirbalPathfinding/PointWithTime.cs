using System;
using System.Collections.Generic;
using System.Text;

namespace GirbalPathfinding
{
    public struct PointWithTime
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int time { get; set; }

        public bool Equals(State state)
        {
            return x == state.x && y == state.y && z == state.z && time == state.time;
        }
    }
}
