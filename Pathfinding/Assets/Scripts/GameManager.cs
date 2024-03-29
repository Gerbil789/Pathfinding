using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var visualizer = AlgorithmVisualizer.Instance;

        Pathfinding.TileVisited += visualizer.VisualizeTile;
        Pathfinding.PathFound += visualizer.VisualizePath;
    }


    public static void PauseGame(float? duration = null)
    {
        if (duration == null) //normal pause/resume
        {
            Time.timeScale = (Time.timeScale == 0) ? 1f : 0f;
        }
        else // pause for a specific duration
        {
            Time.timeScale = 0f; 
            instance.StartCoroutine(ResumeAfterDelay(duration.Value));
        }

    }

    private static IEnumerator ResumeAfterDelay(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f; 
    }
}
