using System;
using System.Collections.Generic;
using System.Text;
using SD.Tools.Algorithmia.Graphs;

namespace GirbalPathfinding
{
    public class Edge : NonDirectedEdge<State>
    {
        public double Length { get; set; }//initilises values
        public double Cost { get; set; }

        public Edge(State node1, State node2) : base(node1, node2)
        {
        }

        public override string ToString()
        {
            return "-> ";// + ConnectedNode.ToString();
        }
    }
}
