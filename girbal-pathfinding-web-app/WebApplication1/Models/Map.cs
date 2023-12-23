using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SD.Tools.Algorithmia.Graphs;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class Map
    {
        public int height { get; set; }
        public int width { get; set; }
        public List<StaticObstacle> staticObstacles { get; set; }

        public Map(int mapWidth, int mapHeight, List<StaticObstacle> staticObstacles)
        {
            height = mapHeight;
            width = mapWidth;
            this.staticObstacles = staticObstacles;
        }
    }
}
