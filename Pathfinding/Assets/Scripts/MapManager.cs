using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public enum TileType {ROCK, GRASS, WOOD}

[System.Serializable]
public class TileTypeToTile
{
    public TileType type;
    public TileBase tile;
}

public class MapManager : MonoBehaviour
{
    public static Vector2Int MapSize = new Vector2Int(32, 32);
    public static bool[,] Map;

    public Vector2Int mapSize = new Vector2Int(64, 64);
    public int seed;

    [SerializeField] TileType currentTileType;

    [SerializeField] public Tilemap tileMap;

    [SerializeField] TileTypeToTile[] tiles;

    private void Awake()
    {
        MapSize = mapSize;
        //seed = PlayerPrefs.GetInt("seed");
        Map = MapGenerator.Generate(MapSize.x, MapSize.y, 4f, seed);

        DrawMap(Map, tileMap);
    }

    public void ChangeTile(Vector3Int clickPos)
    {
        tileMap.SetTile(clickPos, tiles.FirstOrDefault(x => x.type == currentTileType).tile);

        Map[clickPos.x, clickPos.y] = currentTileType == TileType.GRASS;
    }

    public void SelectType(string type){
        currentTileType = (TileType)System.Enum.Parse( typeof(TileType), type );
    }

    public void DrawMap(bool[,] map, Tilemap tilemap)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y])
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles.FirstOrDefault(x => x.type == TileType.GRASS).tile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles.FirstOrDefault(x => x.type == TileType.ROCK).tile);
                }
            }
        }
    }


}
