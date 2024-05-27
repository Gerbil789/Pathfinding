using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pathfinding
{
    protected Vector2Int start, end;
    protected Stack<Vector3Int> path;
    public Queue<Vector2Int> visitedTiles; //for visualization

    public double elapsedTime = 0.0f;

    public abstract Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end);
}
