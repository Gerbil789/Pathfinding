using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Threading;
using static UnityEngine.RuleTile.TilingRuleOutput;
public static class ParallelAstar 
{
    private class Node
    {
        public int G { get; set; }
        public int H { get; set; }
        public int F { get; set; }
        public Node parent { get; set; }
        public Vector3Int position { get; set; }

        public Node(Vector3Int position)
        {
            this.position = position;
        }
    }

    private static Vector3Int start, end;
    private static Stack<Vector3Int> path;
    private static HashSet<Node> openList, closedList;
    private static Dictionary<Vector3Int, Node> allNodes;

    

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
            ParallelAstar.start = start;
            ParallelAstar.end = end;
            path = null;
            openList = new();
            closedList = new();
            allNodes = new();

            AlgorithmVisualizer.Instance.Clear();

            //run algorithm
            Algorithm();

            foreach (var node in allNodes.Values)
            {
                AlgorithmVisualizer.Instance.SetTile(node.position, Color.white);
            }

            if (path != null)
            {
                foreach(var pos in path)
                {
                    AlgorithmVisualizer.Instance.SetTile(pos, Color.blue);
                }
                
            }

            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    static Node GetNode(Vector3Int pos)
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
            ProcessNeighbors(current);
            UpdateCurrentTile( ref current);

            if (current.position == end) 
            {
                path = GeneratePath(current);
            }
        }

        if(path == null)
        {
            Debug.LogWarning("Path not found");
        }
    }

    static void ProcessNeighbors(Node parent)
    {
        Thread[] threads = new Thread[8];
        int i = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int neighborPos = parent.position + new Vector3Int(x, y, 0);
                int threadIndex = i;
                threads[i] = new Thread(() => ExamineNeighbor(neighborPos, parent));
                threads[i].Start();
                i++;
            }
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }
    }

    static void ExamineNeighbor(Vector3Int neighborPos, Node parent)
    {
        if (neighborPos == start || IsOutOfBounds(neighborPos) || !MapManager.Map[neighborPos.x, neighborPos.y])
        {
            return;
        }

        var neighbor = GetNode(neighborPos);

        int gScore = DetermineGScore(neighbor.position, parent.position);

        if (openList.Contains(neighbor))
        {
            if (parent.G + gScore < neighbor.G)
            {
                CalcValues(parent, neighbor, gScore);
            }
        }
        else if (!closedList.Contains(neighbor))
        {
            CalcValues(parent, neighbor, gScore);
            openList.Add(neighbor);
        }
    }

    static bool IsOutOfBounds(Vector3Int pos)
    {
        return pos.x < 0 || pos.y < 0 || pos.x >= MapManager.MapSize.x || pos.y >= MapManager.MapSize.y;
    }

    static int DetermineGScore(Vector3Int neighbor, Vector3Int current)
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

    static void CalcValues(Node parent, Node neighbor, int cost){
        neighbor.parent = parent;
        neighbor.G = parent.G + cost;
        neighbor.H = ((Mathf.Abs((neighbor.position.x - end.x)) + (Mathf.Abs(neighbor.position.y - end.y))) * 10);
        neighbor.F = neighbor.G + neighbor.H;
    }

    static void UpdateCurrentTile(ref Node current){
        openList.Remove(current);
        closedList.Add(current);

        if(openList.Count > 0){
            current = openList.OrderBy(x => x.F).First();
        }
    }

    static Stack<Vector3Int> GeneratePath(Node current){
        Stack<Vector3Int> path = new();

        while(current.position != start){
            path.Push(current.position);
            current = current.parent;
        }
        return path;
    }
}