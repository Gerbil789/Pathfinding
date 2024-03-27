using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public static class Astar 
{
    private static Vector3Int start, end;
    private static Stack<Vector3Int> path;
    private static HashSet<Node> openList, closedList;
    private static bool[,] map;
    private static Dictionary<Vector3Int, Node> allNodes;
    private static Node current;
    public static Stack<Vector3Int> GetPath(Vector3Int _start, Vector3Int _end, bool[,] _map)
    {
        try
        {
            if (_map[_start.x, _start.y] == false)
            {
                throw new ArgumentException("Invalid start position");
            }

            if (_map[_end.x, _end.y] == false)
            {
                throw new ArgumentException("Invalid end position");
            }

            //initialize
            start = _start;
            end = _end;
            map = _map;
            path = null;
            openList = new();
            closedList = new();
            allNodes = new();

            //run algorithm
            current = GetNode(start);
            openList.Add(current);
            Algorithm();

            return path;
        }
        catch (System.Exception ex)
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
        while(openList.Count > 0 && path == null)
        {
            List<Node> neighbors = FindNeighbors(current.position);
            ExamineNeighbors(neighbors, current);
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

    static List <Node> FindNeighbors(Vector3Int parentPos){
        List<Node> neighbors = new List<Node>();
        for(int x = -1 ; x <= 1; x++){
            for(int y = -1 ; y <= 1; y++){
                if(x == 0 && y == 0) continue;

                Vector3Int neighborPos = new Vector3Int(parentPos.x + x, parentPos.y + y, parentPos.z);

                if(neighborPos == start || !map[neighborPos.x, neighborPos.y])
                {
                    continue;
                }
                neighbors.Add(GetNode(neighborPos));
            }
        }
        return neighbors;
    }

    static void ExamineNeighbors(List<Node> neighbors, Node current){
        foreach(var neighbor in neighbors){
            int gScore = DetermineGScore(neighbor.position, current.position);

            if(openList.Contains(neighbor)){
                if(current.G + gScore < neighbor.G){
                    CalcValues(current, neighbor, gScore);
                }
            }else if(!closedList.Contains(neighbor)){
                CalcValues(current, neighbor, gScore);
                openList.Add(neighbor);
            }
        }
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