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
    public TMPro.TMP_Dropdown sizeDropDown;

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
            Debug.LogWarning("No seed!");
            return;
        }
        PlayerPrefs.SetInt("seed", seed);

        if (sizeDropDown.value == 0)
            PlayerPrefs.SetInt("size", (int)SizeEnum.Size_64x64);
        else if(sizeDropDown.value == 1)
            PlayerPrefs.SetInt("size", (int)SizeEnum.Size_128x128);
        else if(sizeDropDown.value == 2)
            PlayerPrefs.SetInt("size", (int)SizeEnum.Size_256x256);
        else if(sizeDropDown.value == 3)
            PlayerPrefs.SetInt("size", (int)SizeEnum.Size_512x512);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
