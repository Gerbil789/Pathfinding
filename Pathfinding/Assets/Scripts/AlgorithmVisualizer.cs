using UnityEngine;
using UnityEngine.Tilemaps;

public class AlgorithmVisualizer : MonoBehaviour
{
    static AlgorithmVisualizer instance;

    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase tile;

    public static AlgorithmVisualizer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AlgorithmVisualizer>();
            }
            return instance;
        }
    }

    public void ShowHide()
    {
        tilemap.gameObject.SetActive(!tilemap.isActiveAndEnabled);
    }

    //public void CreateTiles(HashSet<Node> openList , HashSet<Node> closedList, Vector3Int start, Vector3Int goal, Stack<Vector3Int> path = null)
    //{
    //    foreach(Node node in openList)
    //    {
    //        ColorTile(node.position, orange);
    //    }

    //    foreach(Node node in closedList){
    //        ColorTile(node.position, red);
    //    }

    //    if(path != null){
    //        foreach(Vector3Int pos in path){
    //            if(pos != start && pos != goal){
    //                ColorTile(pos, white);
    //            }
    //        }
    //    }

    //    ColorTile(start, green);
    //    ColorTile(goal, green);
    //}

    public void SetTile(Vector3Int pos, Color color)
    {
        color.a = 0.1f;

        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }





}
