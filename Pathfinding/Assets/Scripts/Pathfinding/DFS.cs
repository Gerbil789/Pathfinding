using System;
using System.Collections.Generic;
using UnityEngine;


public static partial class Pathfinding
{
    public static class DFS
    {
        private static Stack<Vector2Int> stack;
        private static HashSet<Vector2Int> visited;
        public static Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
        {
            try
            {
                if (!IsValidInput(start, end)) return null;

                //initialize
                Pathfinding.start = (Vector2Int)start;
                Pathfinding.end = (Vector2Int)end;

                //run algorithm
                Algorithm();

                return path;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        private static void Algorithm()
        {
            path = null;
            stack = new();
            visited = new();

            stack.Push(start);

            while (stack.Count > 0)
            {
                Vector2Int current = stack.Peek();

                if (current == end)
                {
                    path = new();
                    while (stack.Count > 0)
                    {
                        var pos = stack.Pop();
                        path.Push((Vector3Int)pos);
                    }

                    PathFound?.Invoke(new Stack<Vector3Int>(path));

                    Debug.Log("Path found");
                    return;
                }

                visited.Add(current);


                var next = GetNext(current);
                if (next == null)
                {

                    stack.Pop();
                    continue;
                }


                stack.Push(next.Value);


                TileVisited?.Invoke(current);
            }

            Debug.LogWarning("Path not found");

        }

        static Vector2Int? GetNext(Vector2Int current)
        {
            float minDistance = float.MaxValue;
            Vector2Int? next = null;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector2Int neighbor = current + new Vector2Int(x, y);

                    if (visited.Contains(neighbor) || IsOutOfBounds(neighbor) || MapManager.Map[neighbor.x, neighbor.y] == false)
                        continue;

                    var distance = Vector2Int.Distance(neighbor, end);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        next = neighbor;
                    }
                }
            }

            return next;
        }

        static bool IsOutOfBounds(Vector2Int pos)
        {
            return pos.x < 0 || pos.y < 0 || pos.x >= MapManager.MapSize.x || pos.y >= MapManager.MapSize.y;
        }
    }

}

