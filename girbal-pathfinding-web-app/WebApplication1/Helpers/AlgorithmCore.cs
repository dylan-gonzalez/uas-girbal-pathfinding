using System;
using System.Collections.Generic;
using SD.Tools.Algorithmia.Graphs;
using SD.Tools.Algorithmia.PriorityQueues;

namespace WebApplication1.Helpers
{
    using WebApplication1.Models;
    using WebApplication1.PathPlanners;
    public static class AlgorithmCore
    {
        public static AlgorithmSolution Find (PathfindingSettings settings, int[][] map)
        {
            Console.WriteLine("Finding");
            return Find(settings, map[0].Length, map.Length, GetStaticObstacles(map));
        }

        public static AlgorithmSolution Find(PathfindingSettings settings, int mapWidth, int mapHeight, List<StaticObstacle> staticObstacles)
        {
            var pathPlanners = new List<IPathPlannable>();
            var map = new Map(mapWidth, mapHeight, staticObstacles);

            //initialise all agents
            settings.agents.ForEach(agent =>
            {
                var newAgent = new AStar();
                newAgent.InitialisePathPlanner(map, agent.id, agent.startState, agent.goalState);
                pathPlanners.Add(newAgent);
            });

            //plan each agent's path, starting off with an empty list of constraints
            pathPlanners.ForEach(pathPlanner =>
            {
                pathPlanner.PlanPath(pathPlanner.startState, new List<(State, int)>());
            });

            //get the solution from conducting conflict based search
            var solution = CBS(pathPlanners);

            //convert solution to a list of Agent types
            List<Agent> newSolution = new List<Agent>();
            solution.ForEach(agent =>
            {
                Agent newAgent = new Agent(agent.pathList, agent.agentId);
                newSolution.Add(newAgent);
            });

            return new AlgorithmSolution()
            {
                solution = newSolution,
            };
        }

        public static List<(List<State> pathList, int pathCost, int agentId)> CBS(List<IPathPlannable> pathPlanners)
        {
            
            Comparison<ConstraintNode> comparisonF_n = new Comparison<ConstraintNode>((node2, node1) => node1.cost - node2.cost);

            DirectedGraph<ConstraintNode, DirectedEdge<ConstraintNode>> constraintTree = new DirectedGraph<ConstraintNode, DirectedEdge<ConstraintNode>>();

            var CTOpenList = new SimplePriorityQueue<ConstraintNode>(comparisonF_n);


            var rootCost = 0;
            //var rootSolution = new List<List<State>>();
            var rootSolution = new List<(List<State> list, int cost, int agentId)>();

            for (int i = 0; i < pathPlanners.Count; i++)
            {
                var cost = pathPlanners[i].pathCost;
                rootCost += cost;

                var tuple = (pathPlanners[i].path, cost, pathPlanners[i].id);

                //rootSolution.Add(pathPlanners[i].path);
                rootSolution.Add(tuple);
            }

            var rootNode = new ConstraintNode(new List<(State constraint, int agentId)>(), rootCost, rootSolution);
            constraintTree.Add(rootNode);

            CTOpenList.Add(rootNode);

            do
            {
                var P = CTOpenList.Remove(); //node with lowest cost

                if (P.solution.Count == 1)
                {
                    return P.solution;
                }

                List<Tuple<int, int, State, State>> conflicts = new List<Tuple<int, int, State, State>>();

                //validate paths in P until conflict occurs
                for (int i = 0; i < P.solution.Count; i++)
                {
                    for (int j = i + 1; j < P.solution.Count; j++)
                    {
                        var existingConflict = conflicts.Find(_conflict => _conflict.Item1 == i && _conflict.Item2 == j);

                        if (existingConflict != null)
                            break;

                        var path_i_list = P.solution[i].pathList;
                        var path_j_list = P.solution[j].pathList;

                        var minPathCount = Math.Min(path_i_list.Count, path_j_list.Count);


                        //iterate through each state in the path
                        for (int k = 0; k < minPathCount; k++)
                        {
                            
                            if (path_i_list[k].Equals(path_j_list[k], true))
                            {
                                var conflict = new Tuple<int, int, State, State>(P.solution[i].agentId, P.solution[j].agentId, path_i_list[k], null);
                                conflicts.Add(conflict);
                                break;

                            }

                            /*
                            //Edge conflict
                            if ((path_i_list[k].positionEqual(path_j_list[k+1],true) && path_j_list[k + 1].time == path_i_list[k].time + 1) && (path_j_list[k].positionEqual(path_i_list[k + 1], true) && path_i_list[k + 1].time == path_j_list[k].time + 1))
                            {
                                var conflict = new Tuple<int, int, State, State>(i, j, path_i_list[k], path_j_list[k]); //tuple (a_i, a_j, State_i, State_j) for edge conflict
                                conflicts.Add(conflict);
                                break;
                            }
                            */
                            

                            /*
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

                        List<(List<State> pathList, int pathCost, int agentId)> newSolution = P.solution; //init

                        int agentId = conflicts[c].Item1;
                        if (i != 0)
                            agentId = conflicts[c].Item2;

                        relevantConstraints.Add((new State() { x = conflicts[c].Item3.x, y = conflicts[c].Item3.y, time = conflicts[c].Item3.time }, agentId));

                        int solIndex = P.solution.FindIndex(solution => solution.agentId == agentId); //not sure if this is correct syntax to access agentId from within tuple
                        int plannerIndex = pathPlanners.FindIndex(agent => agent.id == agentId);

                        var tempCost = P.solution[solIndex].pathCost;

                        //replan agent i's path 
                        pathPlanners[plannerIndex].PlanPath(pathPlanners[plannerIndex].startState, relevantConstraints);

                        //if it can't find a path
                        if (pathPlanners[plannerIndex].pathCost >= int.MaxValue)
                            return null;

                        //update the solution
                        newSolution.RemoveAt(solIndex);
                        var newPathTuple = (pathPlanners[plannerIndex].path, pathPlanners[plannerIndex].pathCost, pathPlanners[plannerIndex].id);
                        newSolution.Insert(solIndex, newPathTuple);
                        var solutionCost = P.cost - tempCost + newSolution[solIndex].pathCost;

                        //create a new node in CT
                        var childNode = new ConstraintNode(relevantConstraints, solutionCost, newSolution);
                        constraintTree.Add(childNode);
                        new DirectedEdge<ConstraintNode>(P, childNode);
                        CTOpenList.Add(childNode);
                    }
                }
            } while (true);
        }

        public static List<StaticObstacle> GetStaticObstacles(int[][] map)
        {
            List <StaticObstacle> staticObstacles = new List<StaticObstacle>();

            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == -1)
                    {   //-1 = obstacle, 0 = nothing
                        staticObstacles.Add(new StaticObstacle(x, y));
                    }
                }
            }

            return staticObstacles;
        }
    }
}
