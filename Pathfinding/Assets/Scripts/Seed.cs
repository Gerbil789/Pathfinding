using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Seed : MonoBehaviour
{
    string numbers = "0123456789";
    int charAmount = 6;

    public int seed = 0;
    public string seedText = "000000";
    public TMPro.TMP_InputField inputField;

    void Start()
    {
        RandomSeed();
    }

    public void ChangeSeed(string change)
    {
        seedText = change;
        int.TryParse(change, out seed);
    }


    public void RandomSeed()
    {
        string newSeed = null;
        for(int i = 0; i < charAmount; i++)
        {
            newSeed += numbers[Random.Range(0, numbers.Length)];
        }
        seedText = newSeed;
        inputField.text = seedText;
        int.TryParse(seedText, out seed);
    }

    public void GenerateMap()
    {
        if(seedText == "")
        {
            Debug.Log("No seed!");
            return;
        }
        PlayerPrefs.SetInt("seed", seed);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
