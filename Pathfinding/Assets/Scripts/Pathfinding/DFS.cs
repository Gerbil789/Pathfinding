using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DFS : Pathfinding
{
    private Stack<Vector2Int> stack;
    private HashSet<Vector2Int> visited;
    private Dictionary<Vector2Int, Vector2Int> cameFrom;
    public override Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        try
        {
            visitedTiles = new(); //for visualization

            //initialize
            this.start = (Vector2Int)start;
            this.end = (Vector2Int)end;

            Stopwatch stopwatch = new();
            stopwatch.Start();

            //run algorithm
            Algorithm();

            stopwatch.Stop();
            elapsedTime = stopwatch.Elapsed.TotalMilliseconds;

            return path;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
            return null;
        }
    }

    private void Algorithm()
    {
        path = null;
        stack = new();
        visited = new();
        cameFrom = new();

        stack.Push(start);
        visited.Add(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            if (current == end)
            {
                path = new Stack<Vector3Int>();
                ConstructPath(current); // Reconstruct the path from end to start
                UnityEngine.Debug.Log("Path found");
                return;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                    visited.Add(neighbor);
                    visitedTiles.Enqueue(neighbor); // For visualization
                    cameFrom[neighbor] = current;
                }
            }
        }

        UnityEngine.Debug.LogWarning("Path not found");

    }

    private void ConstructPath(Vector2Int current)
    {
        while (current != start)
        {
            path.Push((Vector3Int)current);
            current = cameFrom[current];
        }
        path.Push((Vector3Int)start);
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the current node itself
                Vector2Int neighbor = new Vector2Int(current.x + x, current.y + y);
                if (!IsOutOfBounds(neighbor) && MapManager.Map[neighbor.x, neighbor.y])
                    neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    bool IsOutOfBounds(Vector2Int pos)
    {
        return pos.x < 0 || pos.y < 0 || pos.x >= MapManager.MapSize.x || pos.y >= MapManager.MapSize.y;
    }
}


