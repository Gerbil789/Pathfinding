using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum Algorithm { ASTAR, ASTAR_PARALLEL }

public class InputManager : MonoBehaviour
{

    private LayerMask layer;
    [SerializeField] Algorithm algorithm;

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

         

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Stack<Vector3Int> path = null;
            switch (algorithm)
            {
                case Algorithm.ASTAR:
                    path = Astar.GetPath(playerPos, clickPos);
                    break;
                case Algorithm.ASTAR_PARALLEL:
                    path = ParallelAstar.GetPath(playerPos, clickPos);
                    break;
            }
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            UnityEngine.Debug.Log("Time taken to calculate path: " + elapsedTime.TotalMilliseconds + " milliseconds");
            if (path != null)
            {
                playerMovement.SetPath(path);
            }

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
}
