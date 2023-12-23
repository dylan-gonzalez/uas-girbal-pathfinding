using System;
using System.Collections.Generic;
using SD.Tools.Algorithmia.PriorityQueues;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public interface IPathPlannable
    {
        List<State> path { get; }
        List<State> closedList { get; set; }
        SimplePriorityQueue<State> openList { get; set; }

        int id { get; set; }
        State startState { get; set; }
        State goalState { get; set; }

        bool isFinished { get; set; }

        List<(State, int)> constraints { get; set; }

        List<State> PlanPath(State requestStart, List<(State, int)> newConstraints);

        int pathCost { get; set; }

        void InitialisePathPlanner(Map map, int plannerIndex, State startState, State goalState);

    }
}