using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

public enum Algorithm { ASTAR, ASTAR_PARALLEL, DFS, BIDIRECTIONAL_ASTAR, DOTS_ASTAR}

public class InputManager : MonoBehaviour
{
    public static event Action<Stack<Vector3Int>> PathCalculated;

    private LayerMask layer;
    public Algorithm strategy;

    private MapManager mapManager;
    private PlayerMovement playerMovement;
    private Camera cam;
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cam = FindObjectOfType<Camera>();
        layer = LayerMask.GetMask("hit");
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
            StartCoroutine(StartPathFinding(playerPos, clickPos));
        }


        // show/hide algorithm visualizer
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            AlgorithmVisualizer.Instance.ShowHide();
        }

        // pause game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.PauseGame();
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
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // calculate path
        var path = algorithm.GetPath(playerPos, clickPos);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Time taken to calculate path: {stopwatch.Elapsed.TotalMilliseconds} milliseconds");

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
            yield return StartCoroutine(AlgorithmVisualizer.Instance.VisualizePath(null, algorithm.visitedTiles));
        }

      
    }
}
