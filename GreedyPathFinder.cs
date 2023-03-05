using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
	public List<Point> FindPathToCompleteGoal(State state)
	{
		var result = new List<Point>();
		int goal = state.Goal;
		var pathfinder = new DijkstraPathFinder();
		var targets = state.Chests.ToHashSet();
		while(goal != 0 && state.Energy > 0)
		{
			var shortestPath = pathfinder.GetPathsByDijkstra(state, result.Count == 0 ? state.Position : result.Last(), targets).FirstOrDefault();
			if (shortestPath == null)
				return new List<Point>();
			result.AddRange(shortestPath.Path.Skip(1));
			targets.Remove(shortestPath.End);
			goal--;
			state.Energy -= shortestPath.Cost;
		}
		if (state.Energy < 0)
			return new List<Point>();
		return result;
	}
}