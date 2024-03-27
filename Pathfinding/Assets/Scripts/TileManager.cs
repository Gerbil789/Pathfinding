using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public enum TileType {ROCK, GRASS, WOOD}

public class TileManager : MonoBehaviour
{
    [SerializeField] TileType tileType;

    Camera cam;
    [SerializeField] LayerMask layer;
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile[] tiles;
    [SerializeField] RuleTile[] ruleTiles;
    [SerializeField] Astar astar;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layer); 
            if(hit.collider != null)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int clickPos = tileMap.WorldToCell(mouseWorldPos);

                ChangeTile(clickPos);
            }
        }   
    }
    void ChangeTile(Vector3Int clickPos)
    {
        
        if (tileType == TileType.ROCK)
        {
            tileMap.SetTile(clickPos, ruleTiles[0]);
            astar.unwalkableTiles.Add(clickPos);
        }

         if (tileType == TileType.WOOD)
        {
            tileMap.SetTile(clickPos, ruleTiles[1]);
            astar.unwalkableTiles.Add(clickPos);
        }

        if (tileType == TileType.GRASS)
        {
            tileMap.SetTile(clickPos, tiles[0]);
            astar.unwalkableTiles.Remove(clickPos);
        }
    }

    public void SelectType(string type){
        tileType = (TileType)System.Enum.Parse( typeof(TileType), type );
    }
   

}
