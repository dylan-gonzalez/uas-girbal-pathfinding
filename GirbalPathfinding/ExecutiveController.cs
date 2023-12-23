using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using SD.Tools.Algorithmia.GeneralDataStructures;
using SD.Tools.Algorithmia.Graphs;
using SD.Tools.Algorithmia.PriorityQueues;

namespace GirbalPathfinding
{
    public class ExecutiveController
    {
        public List<StaticObstacle> staticObstacles { get; set; }

        //public List<Constraint> constraints { get; set; }
        public Map map { get; set; }
        public List<IPathPlannable> pathPlanners { get; set; }
        public int lookahead { get; private set; }
        public int noOfAgents { get; private set; }

        public DirectedGraph<ConstraintNode, DirectedEdge<ConstraintNode>> constraintTree = new DirectedGraph<ConstraintNode, DirectedEdge<ConstraintNode>>();
        public SimplePriorityQueue<ConstraintNode> CTOpenList { get; set; } // CT = constraintTree

        private Comparison<ConstraintNode> comparisonF_n = new Comparison<ConstraintNode>((node2, node1) => node1.cost - node2.cost); 


        public ExecutiveController(int lookahead, int noOfAgents)
        {
            this.lookahead = lookahead;
            staticObstacles = new List<StaticObstacle>();
            pathPlanners = new List<IPathPlannable>(noOfAgents); //new List<>
            map = new Map();
            this.noOfAgents = noOfAgents;

            CTOpenList = new SimplePriorityQueue<ConstraintNode>(comparisonF_n);
        }

        public void initialisePlanner<TPathPlanner>() where TPathPlanner : IPathPlannable, new()
        {
            map.staticObstacles = staticObstacles;
            for (int i = 0; i < noOfAgents; i++)
            {
                Trace.WriteLine("Exec. controller...i: " + i.ToString());

                pathPlanners.Add(new TPathPlanner());
                pathPlanners[i].InitialisePathPlanner(map, staticObstacles, i);
            }
        }

        public int getMapHeight()
        {
            return map.height;
        }

        public int getMapWidth()
        {
            return map.width;
        }

        public int getMapDepth() 
        {
            return map.depth;
        }

        public List<(List<State> pathList, int pathCost)> CBS()
        {
            //CBS is called after planPath()

            var rootCost = 0;
            //var rootSolution = new List<List<State>>();
            var rootSolution = new List<(List<State> list, int cost)>();

            for (int i = 0; i < pathPlanners.Count; i++)
            {
                var cost = pathPlanners[i].pathCost;
                rootCost += cost;

                var tuple = (pathPlanners[i].path, cost);

                //rootSolution.Add(pathPlanners[i].path);
                rootSolution.Add(tuple);
            }

            var rootNode = new ConstraintNode(new List<(State constraint, int agentIndex)>(), rootCost, rootSolution);
            constraintTree.Add(rootNode);   

            CTOpenList.Add(rootNode);

            do
            {
                var P = CTOpenList.Remove(); //node with lowest cost

                //Tuple<int, int, State> conflict; //(agent i index, agent j index, State)

                List<Tuple<int, int, State>> conflicts = new List<Tuple<int, int, State>>();

                //validate paths in P until conflict occurs
                for (int i = 0; i < P.solution.Count; i++)
                {
                    for (int j = i + 1; j < P.solution.Count; j++)
                    {
                        var path_i_list = P.solution[i].pathList;
                        var path_j_list = P.solution[j].pathList;

                        var minPathCount = Math.Min(path_i_list.Count, path_j_list.Count);


                        //iterate through each state in the path
                        for (int k = 0; k < minPathCount; k++)
                        { 
                            if (path_i_list[k].Equals(path_j_list[k], true))
                            {
                                //Trace.WriteLine("Conflict found @: " + path_i_list[k].x.ToString() + " " + path_i_list[k].y.ToString() + " " + path_i_list[k].time.ToString() + " ");
                                //Trace.WriteLine("..between agents: " + i.ToString() + " " + j.ToString());

                                var conflict = new Tuple<int, int, State>(i, j, path_i_list[k]);
                                conflicts.Add(conflict);
                                break;

                            } 

                            /*
                            //Edge conflict
                            if ((path_i_list[k].positionEqual(path_j_list[k+1],true) && path_j_list[k].time == path_i_list[k].time + 1) && (path_j_list[k].positionEqual(path_i_list[k + 1], true) && path_i_list[k].time == path_j_list[k].time + 1))
                            {
                                var conflict = new Tuple<int, int, State, State>(i, j, path_i_list[k], path_j_list[k]); //tuple (a_i, a_j, State_i, State_j) for edge conflict
                                conflicts.Add(conflict);
                                break;
                            }

                            //Diagonal conflict
                            var mid_point_agent1x = path_i_list[k + 1].x - path_i_list[k].x;
                            var mid_point_agent1y = path_i_list[k + 1].y - path_i_list[k].y;
                            var mid_point_agent2x = path_j_list[k + 1].x - path_j_list[k].x;
                            var mid_point_agent2y = path_j_list[k + 1].y - path_j_list[k].y;

                            if ((mid_point_agent1x == mid_point_agent2x) && (mid_point_agent1y == mid_point_agent2y))
                            {
                                var conflict = new Tuple<int, int, State, State>(i, j, path_i_list[k], path_j_list);
                            }
                            */
                        }
                    }
                }

                if (conflicts.Count == 0)
                    return P.solution;


                //iterate through each conflict
                for (int c = 0; c < conflicts.Count; c++)
                {
                    //invoke low level search on both agents involved in the conflict
                    for (int i = 0; i < 2; i++)
                    {
                        //get all the constraints from parent node
                        List<(State, int)> relevantConstraints = new List<(State, int)>();
                        relevantConstraints.AddRange(P.constraints);

                        List<(List<State> pathList, int pathCost)> newSolution = P.solution; //init

                        int index = conflicts[c].Item1;
                        if (i != 0)
                            index = conflicts[c].Item2;

                        relevantConstraints.Add((new State() { x = conflicts[c].Item3.x, y = conflicts[c].Item3.y, time = conflicts[c].Item3.time }, index));

                        var tempCost = P.solution[index].pathCost;

                        //replan agent i's path 
                        pathPlanners[index].PlanPath(map.startStates[index], relevantConstraints);

                        //if it can't find a path
                        if (pathPlanners[index].pathCost >= int.MaxValue)
                            return null;

                        //update the solution
                        newSolution.RemoveAt(index);
                        var newPathTuple = (pathPlanners[index].path, pathPlanners[index].pathCost);
                        newSolution.Insert(index, newPathTuple);
                        var solutionCost = P.cost - tempCost + newSolution[index].pathCost;

                        //create a new node in CT
                        var childNode = new ConstraintNode(relevantConstraints, solutionCost, newSolution);
                        constraintTree.Add(childNode);
                        new DirectedEdge<ConstraintNode>(P, childNode);
                        CTOpenList.Add(childNode);
                    }
                }
              //} while (CTOpenList.Count > 0);
            } while (true);
        }

        public void planPath()
        {
            for (int i = 0; i < noOfAgents; i++)
            {
                if (!pathPlanners[i].isFinished)
                {
                    pathPlanners[i].PlanPath(map.startStates[i], new List<(State, int)>());
                }
            }

            CBS();
        }

        public void moveAlongPath()
        {
            Console.WriteLine("Move ALong Path");

            for (int i = 0; i < noOfAgents; i++)
            {
                map.startStates[i] = new State();

                if (this.pathPlanners[i].path.Count > 0)
                {
                    map.startStates[i].setProperties(pathPlanners[i].path[pathPlanners[i].path.Count - 1]);
                    map.startStates[i].time++;

                }
            }

            //advanceDynamics();

            //map.startState = new State();
            //map.startState.setProperties(pathPlanner.path[pathPlanner.path.Count - 2]);
            //map.startState.time++;
            //advanceDynamics();
        }

        public void setMapDimensions(int x, int y, int z)
        {
            map.width = x;
            map.height = y;
            map.depth = z;
        }

        //public void advanceDynamics()
        //{
        //    foreach (var obs in dynamicObstacles)
        //    {
        //        obs.x = obs.x + obs.speedX;
        //        obs.y = obs.y + obs.speedY;
        //        obs.z = obs.z + obs.speedZ;
        //    }
        //}
    }
}