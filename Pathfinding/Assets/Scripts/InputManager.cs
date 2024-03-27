using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Tilemaps;
using System.Net;
using System;

public class InputManager : MonoBehaviour
{

    [SerializeField] LayerMask layer;

    MapManager mapManager;
    PlayerMovement playerMovement;
    Camera cam;
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        // left click
        if (Input.GetMouseButtonDown(0)) 
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

        // right click
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

            var path = Astar.GetPath(playerPos, clickPos, MapManager.Map);

            if(path != null)
            {
                playerMovement.SetPath(path);
            }

        }
    }
}
