using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    public Tilemap tileMap;
    public RuleTile rock;
    public Tile grass;
    public float scale = 4f;
    [Range(0, 999999)] public int seed = 0;

    public Astar astar;


    int width;
    int height;
    int[,] map;


    float prevScale;
    int prevSeed;


    void Start()
    {
        seed = PlayerPrefs.GetInt("seed");

        width = mapSize.x;
        height = mapSize.y;

        map = PerlinNoise(width, height, scale);
        RenderMap(map, tileMap);
    
        prevScale = scale;
        prevSeed = seed;
    }

    private void Update()
    {
        if(scale != prevScale || seed != prevSeed)
        {
            map = PerlinNoise(width, height, scale);
            RenderMap(map, tileMap);
            prevScale = scale;
        }

        

        
    }


    public  int[,] PerlinNoise(int width, int height, float scale)
    {
        int[,] newMap = new int[width, height];
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (float)x / width * scale;
                float sampleY = (float)y / height * scale;
                int value = Mathf.RoundToInt(Mathf.PerlinNoise(sampleX + seed, sampleY + seed));

                newMap[x, y] = value;
            }
        }
        return newMap;
    }


    public void RenderMap(int[,] map, Tilemap tilemap)
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), rock);
                    astar.unwalkableTiles.Add(new Vector3Int(x,y,0));
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), grass);
                }
            }
        }
    }


    

}


