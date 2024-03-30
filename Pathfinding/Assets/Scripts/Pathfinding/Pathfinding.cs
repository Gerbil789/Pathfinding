using System;
using System.Collections.Generic;
using UnityEngine;

//all pathfinding algorithms will be implemented as static classes within this class
//because static classes cannot inherit from other classes, we will use partial classes to separate the code

//this class contain:
// - the event delegates for the algorithms to communicate with the visualizer
// - start and end positions of the pathfinding algorithm
// - the path found by the algorithm

public static partial class Pathfinding
{
    private static Vector2Int start, end;
    private static Stack<Vector3Int> path;

    public static event Action<Vector2Int> TileVisited;
    public static event Action<Stack<Vector3Int>> PathFound;

    private static bool IsValidInput(Vector3Int start, Vector3Int end)
    {
        if (MapManager.Map[start.x, start.y] == false)
        {
            Debug.LogWarning("Invalid start position");
            return false;
        }

        if (MapManager.Map[end.x, end.y] == false)
        {
            Debug.LogWarning("Invalid end position");
            return false;
        }

        return true;
    }

}
