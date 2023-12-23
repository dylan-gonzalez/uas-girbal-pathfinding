using System;
using System.Collections.Generic;
using SD.Tools.Algorithmia.PriorityQueues;
using System.Text;

namespace GirbalPathfinding
{
    public interface IPathPlannable
    {
        List<State> path { get; } // gets the list of states in the path
        List<State> closedList { get; set; }
        SimplePriorityQueue<State> openList { get; set; }

        bool isFinished { get; set; }

        List<(State, int)> constraints { get; set; }

        List<State> PlanPath(State requestStart,List<(State, int)> newConstraints);

        int pathCost { get; set; }

        void InitialisePathPlanner(Map mp, List<StaticObstacle> staticObs, int plannerIndex);

    }
}