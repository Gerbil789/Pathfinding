using UnityEngine;

public static class Seed 
{
    private static readonly string numbers = "0123456789";
    private static readonly int charAmount = 6;

    public static string GetRandomSeed()
    {
        string newSeed = null;
        for (int i = 0; i < charAmount; i++)
        {
            newSeed += numbers[Random.Range(0, numbers.Length)];
        }
        return newSeed;
    }

}
