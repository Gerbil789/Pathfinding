using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserInterface : MonoBehaviour
{


    [SerializeField] private TMPro.TMP_InputField seedInputField;
    [SerializeField] private Animator mapGeneratorAnimator;

    [SerializeField] private TMPro.TMP_Text avgText;
    [SerializeField] private TMPro.TMP_Text minText;
    [SerializeField] private TMPro.TMP_Text maxText;
    [SerializeField] private TMPro.TMP_Text visitedText;
    [SerializeField] private TMPro.TMP_Text pathLenghtText;
    [SerializeField] private TMPro.TMP_Text pathFoundText;

    [SerializeField] private GameObject mapGenerator;
    [SerializeField] private GameObject statistics;
    [SerializeField] private GameObject quitApp;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            statistics.SetActive(!statistics.activeSelf);
            mapGenerator.SetActive(!mapGenerator.activeSelf);
        }

        #if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitApp.SetActive(true);
        }
        #endif
    }

    public void RandomSeed()
    {
        var seed = Seed.GetRandomSeed();
        seedInputField.text = seed.ToString();
    }


    public void SetMapType(int type)
    {
        if (type == 1)
        {
            mapGeneratorAnimator.SetBool("AdvancedSettings", false);
        }
        else
        {
            mapGeneratorAnimator.SetBool("AdvancedSettings", true);
        }
    }

    public void SetStatistics(double avg, double min, double max, int visited, int pathLenght, bool pathFound)
    {
        avgText.text = "avg. time: " + avg.ToString("F2") + " ms";
        minText.text = "min. time: " + min.ToString("F2") + " ms";
        maxText.text = "max. time: " + max.ToString("F2") + " ms";
        visitedText.text = "visited nodes: " + visited;
        pathLenghtText.text = "path lenght: " + pathLenght;
        pathFoundText.text = pathFound ? "path found" : "path not found";
        pathFoundText.color = pathFound ? Color.green : Color.red;


    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
