using UnityEngine;

public static class MapGenerator
{
    //true = grass, false = rock
    public static bool[,] Generate(int width, int height, float scale, int seed = 0)
    {
        bool[,] map = new bool[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (float)x / width * scale;
                float sampleY = (float)y / height * scale;

                map[x, y] = Mathf.PerlinNoise(sampleX + seed, sampleY + seed) > 0.5f;
            }
        }

        for(int x = width / 2 - 5; x < width / 2 + 5; x++)
        {
            for(int y = height / 2 - 5; y < height / 2 + 5; y++)
            {
                map[x, y] = true;
            }
        }
        return map;
    }
}


