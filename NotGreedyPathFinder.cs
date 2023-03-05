using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
    private DijkstraPathFinder pathfinder = new DijkstraPathFinder();
    private Dictionary<(Point, Point), PathWithCost> pathesBetweenChests = 
        new Dictionary<(Point, Point), PathWithCost>();
    private State state;
    private List<Point> optimalArrangement;
    private int curMaxChest = int.MinValue;
    private int curLowestCost = int.MaxValue;

    public List<Point> FindPathToCompleteGoal(State state)
    {
        this.state = state;
        FindOptimalArrangement(new List<Point>() { state.Position },
            state.Chests.ToList(),
            null, 0);
        return GetOptimalPath();
    }

    private void FindOptimalArrangement(List<Point> arrangement, List<Point> notUsed, Point? toRemove, int curCost)
    {
        if(toRemove != null)
        {
            var key = (arrangement.Last(), toRemove.Value);
            if (!pathesBetweenChests.ContainsKey(key))
            {
                var path = pathfinder.GetPathsByDijkstra(state,
                    arrangement.Last(),
                    new Point[] { toRemove.Value })
                    .FirstOrDefault();
                pathesBetweenChests[key] = path;
            }

            curCost += pathesBetweenChests[key].Cost;
            if (curCost > state.Energy)
            {
                EvaluateArrangement(arrangement, curCost);
                return;
            }

            notUsed.Remove(toRemove.Value);
            arrangement.Add(toRemove.Value);

            if (notUsed.Count == 0)
            {
                EvaluateArrangement(arrangement, curCost);
                return;
            }
        }

        foreach (var chest in notUsed)
        {
            FindOptimalArrangement(new List<Point>(arrangement), new List<Point>(notUsed), chest, curCost);
        }
    }

    private void EvaluateArrangement(List<Point> arrangement, int cost)
    {
        if(arrangement.Count > curMaxChest || (arrangement.Count == curMaxChest && cost < curLowestCost))
        {
            optimalArrangement = arrangement;
            curMaxChest = arrangement.Count;
            curLowestCost = cost;
        }
    }

    private List<Point> GetOptimalPath()
    {
        var path = new List<Point>();
        IEnumerator<Point> first = optimalArrangement.GetEnumerator();
        IEnumerator<Point> second = optimalArrangement.GetEnumerator();
        second.MoveNext();
        while (second.MoveNext() && first.MoveNext())
        {
            path.AddRange(pathesBetweenChests[(first.Current, second.Current)].Path.Skip(1));
        }
        return path;
    }
}