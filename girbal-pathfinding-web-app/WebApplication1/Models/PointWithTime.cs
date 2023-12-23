using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public struct PointWithTime
    {
        public int x { get; set; }
        public int y { get; set; }
        public int time { get; set; }

        public bool Equals(State state)
        {
            return x == state.x && y == state.y && time == state.time;
        }
    }
}
