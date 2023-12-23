using System;
using GirbalPathfinding;
using SD.Tools.Algorithmia.PriorityQueues;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System;
using System.Linq;

public class ConstraintNode
{
	//public int x;
	//public int y;
	//public int z;
	//public int time;

	//constraint --> (a_i,State)

	//public List<State> constraints;
	public List<(State constraint, int agentIndex)> constraints;

	public int cost;
	//public List<List<State>> solution;
	public List<(List<State> pathList, int pathCost)> solution;

	public ConstraintNode(List<(State constraint, int agentIndex)> constraints, int cost, List<(List<State> list, int cost)> solution)
	{
		this.constraints = constraints;
		this.cost = cost;
		this.solution = solution;
	}
}
