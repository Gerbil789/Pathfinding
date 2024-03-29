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

    public void SetTile(Vector3Int pos, Color color, float alpha = 0.1f)
    {
        color.a = alpha;

        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
