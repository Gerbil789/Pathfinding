using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System;

public enum TileType {ROCK, GRASS}
public enum SizeEnum
{
    Size_64x64,
    Size_128x128,
    Size_256x256,
    Size_512x512
}

public enum MapType
{
    NOISE,
    MAZE,
}

[System.Serializable]
public class TileTypeToTile
{
    public TileType type;
    public TileBase tile;
}

public class MapManager : MonoBehaviour
{
    public static Vector2Int MapSize = new Vector2Int(64, 64);
    public static bool[,] Map;

    [SerializeField] SizeEnum mapSize = SizeEnum.Size_64x64;
    [SerializeField] MapType mapType = MapType.NOISE;

    public int seed = 0;
    public float scale = 16f;
    public float amplitude = 0.4f;


    [SerializeField] TileType currentTileType;

    [SerializeField] public Tilemap tileMap;

    [SerializeField] TileTypeToTile[] tiles;


    private void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        tileMap.ClearAllTiles();

        MapSize = GetSize(mapSize);
        Map = mapType switch
        {
            MapType.NOISE => MapGenerator.GenerateNoise(MapSize.x, MapSize.y, MapSize.x / 64 * scale, amplitude, seed),
            MapType.MAZE => MapGenerator.GenerateMaze(MapSize.x, MapSize.y, seed),
            _ => MapGenerator.GenerateNoise(MapSize.x, MapSize.y, scale, amplitude, seed)
        };
        
        var visualizer = FindObjectOfType<AlgorithmVisualizer>();
        visualizer.Stop();

        var player = FindObjectOfType<PlayerMovement>();
        player.transform.position = new Vector3(MapSize.x / 2, MapSize.y / 2, 0);
        FindObjectOfType<Camera>().transform.position = new Vector3(MapSize.x / 2, MapSize.y / 2, -10);

        DrawMap(Map, tileMap);
    }

    public void SetMapSize(int size)
    {
        mapSize = (SizeEnum)size;

    }

    public void SetSeed(string seed)
    {
        int.TryParse(seed, out this.seed);
    }

    public void SetMapType(int type)
    {
        mapType = (MapType)type;
    }

    public void SetScale(string scale)
    {
        float.TryParse(scale, out this.scale);
    }

    public void SetAmplitude(string amplitude)
    {
        float.TryParse(amplitude, out this.amplitude);
    }

    public void ClearMap()
    {
        tileMap.ClearAllTiles();
        Map = null;
    }

    public void ChangeTile(Vector3Int clickPos)
    {
        tileMap.SetTile(clickPos, tiles.FirstOrDefault(x => x.type == currentTileType).tile);
        tileMap.SetColor(clickPos, Color.white);

        Map[clickPos.x, clickPos.y] = currentTileType == TileType.GRASS;
    }

    public void SelectType(string type){
        currentTileType = (TileType)System.Enum.Parse( typeof(TileType), type);
    }

    public void SetTileType(Int32 type)
    {
        currentTileType = (TileType)type;
    }

    public void DrawMap(bool[,] map, Tilemap tilemap)
    {
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
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


