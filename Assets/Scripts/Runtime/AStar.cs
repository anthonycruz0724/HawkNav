using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    // Returns a list of Vector3 points along the path
    public static List<Vector3> FindPath(Node start, Node goal)
    {
        if (start == null || goal == null) return new List<Vector3>();

        var openSet = new List<Node> { start };
        var cameFrom = new Dictionary<Node, Node>();
        var gScore = new Dictionary<Node, float> { [start] = 0f };
        var fScore = new Dictionary<Node, float> { [start] = Heuristic(start, goal) };

        while (openSet.Count > 0)
        {
            // Get node with smallest fScore
            Node current = openSet[0];
            float bestF = fScore[current];
            for (int i = 1; i < openSet.Count; i++)
            {
                float f = fScore.TryGetValue(openSet[i], out var v) ? v : Mathf.Infinity;
                if (f < bestF)
                {
                    bestF = f;
                    current = openSet[i];
                }
            }

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                if (neighbor == null) continue;
                float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (!gScore.TryGetValue(neighbor, out var gOld) || tentativeG < gOld)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3>(); // no path found
    }

    static float Heuristic(Node a, Node b)
        => Vector3.Distance(a.transform.position, b.transform.position);

    static List<Vector3> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        var totalPath = new List<Vector3> { current.transform.position };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current.transform.position);
        }
        return totalPath;
    }
}
