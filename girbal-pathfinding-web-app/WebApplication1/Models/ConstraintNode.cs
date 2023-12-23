using System;
using SD.Tools.Algorithmia.PriorityQueues;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Linq;
using WebApplication1.Models;

public class ConstraintNode
{
	public List<(State constraint, int agentIndex)> constraints;

	public int cost;
	public List<(List<State> pathList, int pathCost, int agentId)> solution;

	public ConstraintNode(List<(State constraint, int agentId)> constraints, int cost, List<(List<State> list, int cost, int agentId)> solution)
	{
		this.constraints = constraints;
		this.cost = cost;
		this.solution = solution;
	}
}
