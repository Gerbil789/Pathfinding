using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization;

public enum TileType {ROCK, GRASS, WOOD}
public enum SizeEnum
{
    Size_64x64,
    Size_128x128,
    Size_256x256,
    Size_512x512
}

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

    //public Vector2Int mapSize = new Vector2Int(64, 64);

    [SerializeField] SizeEnum mapSize = SizeEnum.Size_64x64;

    public int seed;
    public float scale = 4f;
    public float amplitude = 0.5f;

    [SerializeField] TileType currentTileType;

    [SerializeField] public Tilemap tileMap;

    [SerializeField] TileTypeToTile[] tiles;

    private void Awake()
    {
        seed = PlayerPrefs.GetInt("seed", seed);
        mapSize = (SizeEnum)PlayerPrefs.GetInt("size", (int)mapSize);

        if (Map == null) 
            GenerateMap();
    }

    public void GenerateMap()
    {
        tileMap.ClearAllTiles();

        MapSize = GetSize(mapSize);
        Map = MapGenerator.Generate(MapSize.x, MapSize.y, MapSize.x / 64 * scale, amplitude, seed);
        DrawMap(Map, tileMap);
    }

    public void ClearMap()
    {
        tileMap.ClearAllTiles();
        Map = null;
    }

    public void ChangeTile(Vector3Int clickPos)
    {
        tileMap.SetTile(clickPos, tiles.FirstOrDefault(x => x.type == currentTileType).tile);

        Map[clickPos.x, clickPos.y] = currentTileType == TileType.GRASS;
    }

    public void SelectType(string type){
        currentTileType = (TileType)System.Enum.Parse( typeof(TileType), type);
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
    Vector2Int GetSize(SizeEnum sizeEnum) =>
        sizeEnum switch
        {
            SizeEnum.Size_64x64 => new Vector2Int(64, 64),
            SizeEnum.Size_128x128 => new Vector2Int(128, 128),
            SizeEnum.Size_256x256 => new Vector2Int(256, 256),
            SizeEnum.Size_512x512 => new Vector2Int(512, 512),
            _ => new Vector2Int(64, 64) // Default size
        };

}


