using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarDebugger : MonoBehaviour
{
    static AstarDebugger instance;

    public static AstarDebugger MyInstance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<AstarDebugger>();
            }

            return instance;
        }
    }
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile tile;

    [SerializeField] Color red, green, white, orange;
   

    public void CreateTiles(HashSet<Node> openList , HashSet<Node> closedList, Vector3Int start, Vector3Int goal, Stack<Vector3Int> path = null)
    {
        foreach(Node node in openList)
        {
            ColorTile(node.position, orange);
        }

        foreach(Node node in closedList){
            ColorTile(node.position, red);
        }

        if(path != null){
            foreach(Vector3Int pos in path){
                if(pos != start && pos != goal){
                    ColorTile(pos, white);
                }
            }
        }

        ColorTile(start, green);
        ColorTile(goal, green);
    }

    public void ColorTile(Vector3Int pos, Color col)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, col);
    }

    public void ShowHide(){
        tilemap.gameObject.SetActive(!tilemap.isActiveAndEnabled);
    }

    public void ClearTiles(Dictionary<Vector3Int, Node> allNodes){
        foreach(Vector3Int pos in allNodes.Keys){
            tilemap.SetTile(pos, null);
        }
    }
}
