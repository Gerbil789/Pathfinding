using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static partial class Pathfinding
{
    public static class Astar
    {
        private class Node
        {
            public int G { get; set; }
            public int H { get; set; }
            public int F { get; set; }
            public Node parent { get; set; }
            public Vector2Int position { get; set; }
            public Node(Vector2Int position) => this.position = position;
        }

        private static HashSet<Node> openList, closedList;
        private static Dictionary<Vector2Int, Node> allNodes;

        public static Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
        {
            try
            {
                if (MapManager.Map[start.x, start.y] == false)
                {
                    Debug.LogWarning("Invalid start position");
                    return null;
                }

                if (MapManager.Map[end.x, end.y] == false)
                {
                    Debug.LogWarning("Invalid end position");
                    return null;
                }

                //initialize
                Pathfinding.start = (Vector2Int)start;
                Pathfinding.end = (Vector2Int)end;
                path = null;
                openList = new();
                closedList = new();
                allNodes = new();

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
        static Node GetNode(Vector2Int pos)
        {
            if (allNodes.ContainsKey(pos))
            {
                return allNodes[pos];
            }
            else
            {
                Node node = new Node(pos);
                allNodes.Add(pos, node);
                return node;
            }
        }
        static void Algorithm()
        {
            var current = GetNode(start);
            openList.Add(current);

            while (openList.Count > 0 && path == null)
            {
                List<Node> neighbors = FindNeighbors(current.position);
                ExamineNeighbors(neighbors, current);
                UpdateCurrentTile(ref current);

                if (current.position == end)
                {
                    path = GeneratePath(current);
                    PathFound.Invoke(new Stack<Vector3Int>(path)); //copy path to avoid modifying the original
                }
            }

            if (path == null)
            {
                Debug.LogWarning("Path not found");
            }
        }
        static List<Node> FindNeighbors(Vector2Int parentPos)
        {
            List<Node> neighbors = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector2Int neighborPos = new Vector2Int(parentPos.x + x, parentPos.y + y);

                    if (neighborPos == start || IsOutOfBounds(neighborPos) || !MapManager.Map[neighborPos.x, neighborPos.y])
                    {
                        continue;
                    }

                    neighbors.Add(GetNode(neighborPos));
                }
            }
            return neighbors;
        }
        static bool IsOutOfBounds(Vector2Int pos)
        {
            return pos.x < 0 || pos.y < 0 || pos.x >= MapManager.MapSize.x || pos.y >= MapManager.MapSize.y;
        }
        static void ExamineNeighbors(List<Node> neighbors, Node current)
        {
            foreach (var neighbor in neighbors)
            {
                int gScore = DetermineGScore(neighbor.position, current.position);

                if (openList.Contains(neighbor))
                {
                    if (current.G + gScore < neighbor.G)
                    {
                        CalcValues(current, neighbor, gScore);
                    }
                }
                else if (!closedList.Contains(neighbor))
                {
                    CalcValues(current, neighbor, gScore);
                    openList.Add(neighbor);
                    TileVisited?.Invoke(neighbor.position);
                }


            }
        }
        static int DetermineGScore(Vector2Int neighbor, Vector2Int current)
        {
            int gScore;
            int x = current.x - neighbor.x;
            int y = current.y - neighbor.y;
            if (Mathf.Abs(x - y) % 2 == 1)
            {
                gScore = 10; //horizontal or vertical
            }
            else
            {
                gScore = 14; //diagonal
            }
            return gScore;
        }
        static void CalcValues(Node parent, Node neighbor, int cost)
        {
            neighbor.parent = parent;
            neighbor.G = parent.G + cost;
            neighbor.H = ((Mathf.Abs((neighbor.position.x - end.x)) + (Mathf.Abs(neighbor.position.y - end.y))) * 10);
            neighbor.F = neighbor.G + neighbor.H;
        }
        static void UpdateCurrentTile(ref Node current)
        {
            openList.Remove(current);
            closedList.Add(current);

            if (openList.Count > 0)
            {
                current = openList.OrderBy(x => x.F).First();
            }
        }
        static Stack<Vector3Int> GeneratePath(Node current)
        {
            Stack<Vector3Int> path = new();

            // push the end node
            path.Push((Vector3Int)current.position);
            //var dir = current.position - current.parent.position;
            current = current.parent;

            // push the rest of the path 
            while (current.position != start)
            {
                //var newDir = current.position - current.parent.position;

                ////push only if direction changes
                //if(newDir != dir)
                //{
                //    path.Push((Vector3Int)current.position);
                //    dir = newDir;
                //}
                path.Push((Vector3Int)current.position);
                current = current.parent;
            }

            // push the start node
            path.Push((Vector3Int)current.position);
            return path;
        }
    }
}
