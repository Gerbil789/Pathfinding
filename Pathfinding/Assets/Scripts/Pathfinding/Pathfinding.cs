using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pathfinding
{
    protected Vector2Int start, end;
    protected Stack<Vector3Int> path;
    public Queue<Vector2Int> visitedTiles; //for visualization

    protected bool IsValidInput(Vector3Int start, Vector3Int end)
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

    public abstract Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end);
}
