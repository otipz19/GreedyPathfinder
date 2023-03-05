using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

internal class DijkstraData
{
	public Point? Parent { get; set; }
	public int Cost { get; set; }
}

internal static class PointExtensions
{
	public static IEnumerable<Point> GetNeighbours(this Point point)
	{
		yield return new Point(point.X + 1, point.Y);
        yield return new Point(point.X - 1, point.Y);
        yield return new Point(point.X, point.Y + 1);
        yield return new Point(point.X, point.Y - 1);
    }
}

public class DijkstraPathFinder
{
    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
        IEnumerable<Point> targets)
    {
        var opened = new HashSet<Point>();
        var track = new Dictionary<Point, DijkstraData>();
        track[start] = new DijkstraData { Parent = null, Cost = 0 };

        while (true)
        {
            if (targets.Count() == 0)
                break;
            Point? toOpen = GetPointToOpen(track, opened);
            if (toOpen == null)
                break;

            if (targets.Contains(toOpen.Value))
            {
                targets = targets.Where(p => p != toOpen);
                yield return new PathWithCost(track[toOpen.Value].Cost,
                    GetPathIterator(track, toOpen.Value).Reverse().ToArray());
            }

            opened.Add(toOpen.Value);
            foreach (var neighbour in toOpen.Value.GetNeighbours().Where(n => state.InsideMap(n) && !state.IsWallAt(n)))
            {
                var newCost = track[toOpen.Value].Cost + state.CellCost[neighbour.X, neighbour.Y];
                if (!track.ContainsKey(neighbour) || track[neighbour].Cost > newCost)
                {
                    track[neighbour] = new DijkstraData { Parent = toOpen, Cost = newCost };
                }
            }
        }
    }

    private static IEnumerable<Point> GetPathIterator(Dictionary<Point, DijkstraData> track, Point target)
    {
        for (Point? point = target; point != null; point = track[point.Value].Parent)
        {
			yield return point.Value;
        }
    }

    private static Point? GetPointToOpen(Dictionary<Point, DijkstraData> track, HashSet<Point> opened)
    {
        Point? toOpen = null;
        int bestCost = int.MaxValue;
        foreach (var point in track.Keys.Where(p => !opened.Contains(p)))
        {
            if (track[point].Cost < bestCost)
            {
                toOpen = point;
                bestCost = track[point].Cost;
            }
        }
        return toOpen;
    }
}