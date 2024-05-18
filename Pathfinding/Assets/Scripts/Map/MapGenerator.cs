using UnityEngine;

public static class MapGenerator
{
    //true = grass, false = rock
    public static bool[,] GenerateNoise(int width, int height, float scale, float amplitude, int seed = 0)
    {
        bool[,] map = new bool[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (float)x / width * scale;
                float sampleY = (float)y / height * scale;

                map[x, y] = Mathf.PerlinNoise(sampleX + seed, sampleY + seed) > amplitude;
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


    //recursive backtracking algorithm
    public static bool[,] GenerateMaze(int width, int height, int seed = 0)
    {
        bool[,] map = new bool[width, height];
        System.Random rand = new System.Random(seed);

        // Initialize the map, with walls everywhere
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = false;

        // Recursive method to carve paths
        void CarvePath(int x, int y)
        {
            // Direction vectors
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            int[] directions = { 0, 1, 2, 3 };
            Shuffle(directions, rand);

            // Explore neighbors in random order
            for (int i = 0; i < directions.Length; i++)
            {
                int nx = x + 2 * dx[directions[i]];
                int ny = y + 2 * dy[directions[i]];

                // See if the neighbor is valid and has all walls intact
                if (IsInBounds(nx, ny, width, height) && !map[nx, ny])
                {
                    map[nx - dx[directions[i]], ny - dy[directions[i]]] = true; // Carve through wall
                    map[nx, ny] = true; // Carve into new cell
                    CarvePath(nx, ny);
                }
            }
        }

        // Start from the top-left corner, or any other corner
        map[1, 1] = true;
        CarvePath(1, 1);

        for (int x = width / 2 - 5; x < width / 2 + 5; x++)
        {
            for (int y = height / 2 - 5; y < height / 2 + 5; y++)
            {
                map[x, y] = true;
            }
        }

        return map;
    }

    private static bool IsInBounds(int x, int y, int width, int height)
    {
        return x > 0 && x < width && y > 0 && y < height;
    }

    private static void Shuffle(int[] array, System.Random rand)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}


