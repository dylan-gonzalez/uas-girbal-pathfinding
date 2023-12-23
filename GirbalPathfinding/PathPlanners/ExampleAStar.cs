using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SD.Tools.Algorithmia.PriorityQueues;

namespace GirbalPathfinding
{
    internal class ExampleAStar : IPathPlannable
    {
        private List<State> stateList;
        private Map map;
        private List<double> hTableStatic;

        private double herr, qtime_avg;

        private SimplePriorityQueue<State> globalOpenList;
        private SimplePriorityQueue<State> goalQueue;

        private Comparison<State> comparisonF_n = new Comparison<State>((item2, item1) => item1.comparisonFhat - item2.comparisonFhat);

        public List<State> lookaheadRegion { get => globalClosedList; }

        public List<State> globalClosedList { get; set; }
        public double Collision_cost { get; private set; }
        public List<State> path { get; set; }

        public List<Constraint> constraints { get; set; }
        public int droneRadius;
        //set droneState to middle state.. with conflicts, check if obstacle is within radius

        public int plannerIndex { get; set; }

        public ExampleAStar()
        {
            path = new List<State>();

            globalOpenList = new SimplePriorityQueue<State>(comparisonF_n);
            goalQueue = new SimplePriorityQueue<State>(comparisonF_n);

            globalClosedList = new List<State>();
            constraints = new List<Constraint>();
            Collision_cost = 1000;

            droneRadius = 1;
        }

        public void InitialisePathPlanner(Map mp, List<StaticObstacle> staticObs, int plannerIndex)
        {
            stateList = new List<State>();
            map = mp;
            map.OnGoalUpdate += UpdateGoal;
            this.plannerIndex = plannerIndex;

            hTableStatic = new List<double>(map.width * map.height);
        }


        public List<State> PlanPath(State requestStart)
        {
            path.Clear();
            AStarSearch(requestStart);

            if (globalOpenList.Count == 0)
                return null;

            var sgoal = PickBestState();
            var state = sgoal;

            
            while (state != map.startStates[plannerIndex])
            {
                path.Add(state);
                state = state.parent;
            }
            

            //Trace.WriteLine("Example A*: " + this.plannerIndex.ToString() + "; " + path.Count.ToString());
            //Trace.WriteLine(path.)
            //Console.WriteLine(this.plannerIndex.ToString(), path.Count);

            return path;
        }

        public int AStarSearch(State startState)
        {
            var currentState = startState;

            //Go through all states and prune/set starting
            foreach (var state in stateList.ToList())
            {
                //Time has passed, delete it
                if (state.time < startState.time)
                {
                    stateList.Remove(state);
                }
            }
            //return 0;

            //clear open/close
            globalOpenList.Clear();
            var openCheck = new List<State>(stateList.Count);
            globalClosedList.Clear();

            //Set start state to 0/defaults
            startState.g = 0;

            Console.WriteLine(TransformCoordToIndex(startState.x, startState.y));
            startState.h = hTableStatic[TransformCoordToIndex(startState.x, startState.y)];
            startState.generateComparisonFhat();

            //push start into open then start expanding
            globalOpenList.Add(startState);
            openCheck.Add(startState);
            double hsum = 0;

            do
            {
                var state = globalOpenList.Remove();

                state.generateComparisonFhat();

                globalClosedList.Add(state);
                openCheck.Remove(state);
                State bestChild = null;

                //Goto all nearby nodes
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //find child state
                        var childState = stateList.Find((st) => st.x == state.x + i && st.y == state.y + j && st.time == state.time + 1);

                        //check to make sure it isnt a static obstacle and constraint
                        if (!(childState is null) && (childState.checkIfStaticObstacle(map.staticObstacles) || childState.checkIfConstraint(this.constraints)))
                        {
                            //Trace.WriteLine("AVOID");
                            continue;
                        }
                        //Check object exists and is not on closed list
                        if (childState is null || !globalClosedList.Exists(st => childState.Equals(st, false)))
                        {
                            //If state does not exist then make it
                            if (childState is null)
                            {
                                childState = new State();
                                //Set properties and if not valid, continue
                                if (!childState.setProperties(state.x + i, state.y + j, state.time + 1, map.width, map.height))
                                    continue;

                                //Set other properties
                                childState.g = double.MaxValue;
                                childState.h = 0;
                                childState.frontier_time = state.time + 1;

                                childState.generateComparisonFhat();

                                CreatePredecessors(childState, state.time);

                                /*
                                if (plannerIndex == 1)
                                    Trace.WriteLine("Child state: (" + childState.x.ToString() + ", " + childState.y.ToString() + ", " + childState.time.ToString() + ")");
                                */
                                stateList.Add(childState);
                            }

                            //Check to see if this is the goal
                            if (childState.positionEqual(map.goalStates[plannerIndex]))
                                return 0;

                            childState.h = hTableStatic[TransformCoordToIndex(childState.x, childState.y)];
                            childState.generateComparisonFhat();
                            double scost = state.g + staticCost(state, childState);

                            //Check if child costs are larger than calculated costs and if so, set the correct costs
                            if (childState.g > scost)
                            {
                                childState.g = scost;
                                childState.parent = state;

                                childState.generateComparisonFhat();

                                //add to open list if it's not on there
                                if (!openCheck.Contains(childState))
                                {
                                    globalOpenList.Add(childState);
                                    openCheck.Add(childState);
                                }
                            }
                        }
                        //Set best child
                        if (bestChild is null || bestChild.f_static() > childState.f_static())
                            bestChild = childState;
                    }
                }
                if (!(bestChild is null))
                {
                    double fdiff = bestChild.f_static() - state.f_static();
                    fdiff = (fdiff < 0) ? 0 : fdiff;
                    hsum += fdiff;
                }
            } while (globalOpenList.Count != 0);
            return 0;
        }

        public void UpdateGoal(object obj, StateUpdateEventArgs e)
        {
            if (e.plannerIndex != plannerIndex)
                return;

            //create a static lookup table to speed up creation
            bool[] staticObstacleLUT = Enumerable.Repeat(false, map.height * map.width).ToArray();
            //staticObstacleLUT.AddRange(Enumerable.Repeat(false, map.height * map.width));
            hTableStatic.AddRange(Enumerable.Repeat(0d, map.height * map.width));
            //d_errTable.AddRange(Enumerable.Repeat(0d, map.height * map.width));
            //dTable.AddRange(Enumerable.Repeat(0d, map.height * map.width));
            foreach (var obs in map.staticObstacles)
            {
                staticObstacleLUT[TransformCoordToIndex(obs.x, obs.y)] = true;
            }

            //var kernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, int, int>(InitUpdateLUTKernel);
            //var staticObsKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView2D<double>, ArrayView2D<int>>(SetStaticMaxFromLUT);

            //using (var distanceBuffer = accelerator.Allocate<double>(map.width, map.height))
            //using (var staticObsLutBuffer = accelerator.Allocate<int>(map.width, map.height))
            //{
            //    staticObsLutBuffer.CopyFrom(staticObstacleLUT, 0, LongIndex2.Zero, map.width * map.height);
            //    //Array.Copy(staticObstacleLUT, , map.width*map.height);

            //    kernel(distanceBuffer.Extent, distanceBuffer.View, map.goalState.x, map.goalState.y);
            //    staticObsKernel(distanceBuffer.Extent, distanceBuffer.View, staticObsLutBuffer.View);
            //    accelerator.Synchronize();

            //    // Resolve data
            //    var data = distanceBuffer.GetAsArray();
            //    hTableStatic.AddRange(data);
            //    d_errTable.AddRange(data);
            //    dTable.AddRange(data);
            //}

            //Run a new htableupdate
            for (int i = 1; i < map.height; i++)
            {
                for (int j = 0; j < map.width; j++)
                {
                    var index = TransformCoordToIndex(j, i);
                    //Check if in this coordinate, there is a static obs
                    if (staticObstacleLUT[index])
                    {
                        //set hval as maxvalue
                        hTableStatic.Insert(index, double.MaxValue);
                    }
                    else
                    {
                        //No static obs
                        var dist = map.goalStates[plannerIndex].distanceTo(j, i);
                        hTableStatic.Insert(index, dist);
                    }
                }
            }
        }

        private State PickBestState()
        {
            foreach (var st in globalOpenList)
            {
                goalQueue.Add(st);
            }
            globalOpenList.Clear();

            var state = goalQueue.Peek();
            do
            {
                var peekState = goalQueue.Peek();
                if (state.time <= peekState.time && state.h > peekState.h)
                {
                    state = goalQueue.Peek();
                }
                globalOpenList.Add(goalQueue.Remove());
            } while (goalQueue.Count != 0);

            return state;
        }

        private int TransformCoordToIndex(int x, int y)
        {
            return map.height * y + x;
        }

        private void CreatePredecessors(State state, int time)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    state.predecessors.Add(new PointWithTime() { x = state.x + i, y = state.y + j, time = time });
                }
            }
        }

        /// <summary>
        /// Returns a movement cost unless the goal is moving to the goal
        /// </summary>
        /// <param name="state"></param>
        /// <param name="childState"></param>
        /// <returns></returns>
        private double staticCost(State state, State childState)
        {
            if (state.positionEqual(map.goalStates[plannerIndex]) && childState.positionEqual(map.goalStates[plannerIndex]))
                return 0;
            return 1;
        }
    }
}
