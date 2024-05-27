using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum Algorithm { ASTAR, DOTS_ASTAR, DFS, ASTAR_PARALLEL, BIDIRECTIONAL_ASTAR}

public class InputManager : MonoBehaviour
{
    public static event Action<Stack<Vector3Int>> PathCalculated;

    private LayerMask layer;
    public Algorithm strategy;

    private MapManager mapManager;
    private PlayerMovement playerMovement;
    private Camera cam;
    private UserInterface userInterface;

    public int runs = 50;
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cam = FindObjectOfType<Camera>();
        layer = LayerMask.GetMask("hit");
        userInterface = FindObjectOfType<UserInterface>();
    }

    void Update()
    {
        // left hold -> change terrain
        if (Input.GetMouseButton(0)) 
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layer);
            if (hit.collider == null)
            {
                return;
            }

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickPos = mapManager.tileMap.WorldToCell(mouseWorldPos);

            mapManager.ChangeTile(clickPos);
        }

        // right click -> move player
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layer);
            if (hit.collider == null)
            {
                return;
            }

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int clickPos = mapManager.tileMap.WorldToCell(mouseWorldPos);

            var playerPos = mapManager.tileMap.WorldToCell(playerMovement.transform.position);
            if (playerPos == clickPos) return;
            if (!IsValidInput(playerPos, clickPos)) return;

            StartCoroutine(StartPathFinding(playerPos, clickPos));
        }
    }

    IEnumerator StartPathFinding(Vector3Int playerPos, Vector3Int clickPos)
    {
        AlgorithmVisualizer.Instance.Clear();


        Pathfinding algorithm = strategy switch 
        { 
            Algorithm.ASTAR => new Astar(), 
            Algorithm.ASTAR_PARALLEL => new ParallelAstar(), 
            Algorithm.DFS => new DFS(), 
            Algorithm.BIDIRECTIONAL_ASTAR => new BidirectionalAstar(), 
            Algorithm.DOTS_ASTAR => new DOTS_Astar(),
            _ => throw new Exception("Invalid algorithm")
        };

        Stack<Vector3Int> path = null;

        double averageTime = 0;
        double minTime = double.MaxValue;
        double maxTime = double.MinValue;

        for(int i = 0; i < runs; i++)
        {
            // calculate path
            path = algorithm.GetPath(playerPos, clickPos);

            averageTime += algorithm.elapsedTime;
            minTime = Math.Min(minTime, algorithm.elapsedTime);
            maxTime = Math.Max(maxTime, algorithm.elapsedTime);
        }

        userInterface.SetStatistics(averageTime / runs, minTime, maxTime, algorithm.visitedTiles.Count, path?.Count ?? 0, path != null);

        if (path != null)
        {
            // reverse path
            Stack<Vector3Int> reversedPath = new Stack<Vector3Int>();
            foreach (var pos in path)
            {
                reversedPath.Push(pos);
            }

            // visualize path
            yield return StartCoroutine(AlgorithmVisualizer.Instance.VisualizePath(reversedPath, algorithm.visitedTiles));

            // move player
            PathCalculated?.Invoke(reversedPath);
        }
        else
        {
            //visualize path (not found)
            yield return StartCoroutine(AlgorithmVisualizer.Instance.VisualizePath(null, algorithm.visitedTiles));
        }
    }

    private bool IsValidInput(Vector3Int start, Vector3Int end)
    {
        if (MapManager.Map[start.x, start.y] == false)
        {
            UnityEngine.Debug.LogWarning("Invalid start position");
            return false;
        }

        if (MapManager.Map[end.x, end.y] == false)
        {
            UnityEngine.Debug.LogWarning("Invalid end position");
            return false;
        }

        return true;
    }


    public void SetRuns(string value)
    {
        runs = int.Parse(value);
    }
}
