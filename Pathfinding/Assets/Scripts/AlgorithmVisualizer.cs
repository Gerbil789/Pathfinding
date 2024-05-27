using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AlgorithmVisualizer : MonoBehaviour
{
    [SerializeField] private bool visualize = true;
    static AlgorithmVisualizer instance;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private float visualizationDelay = 0.05f;
    [SerializeField] private float pathDelay = 0.05f;


    //public static event Action<Stack<Vector3Int>> VisualizationFinished;

    struct VisualizationData
    {
        public Vector3Int position;
        public Color color;
        public VisualizationData(Vector3Int position, Color color, float? alpha)
        {
            this.position = position;
            this.color = color;
            this.color.a = alpha ?? 1f;
        }
    }


    public static AlgorithmVisualizer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AlgorithmVisualizer>();
            }
            return instance;
        }
    }

    public void ShowHide()
    {
        tilemap.gameObject.SetActive(!tilemap.isActiveAndEnabled);
    }

    public IEnumerator VisualizePath(Stack<Vector3Int> path, Queue<Vector2Int> visitedTiles)
    {
        if (!visualize)
        {
            Stop();
            yield break; // stop the coroutine
        }


        Queue<Vector3Int> convertedQueue = new Queue<Vector3Int>();
        foreach (Vector2Int vector2Int in visitedTiles)
        {
            Vector3Int vector3Int = new Vector3Int(vector2Int.x, vector2Int.y, 0);
            convertedQueue.Enqueue(vector3Int);
        }

        yield return StartCoroutine(VisualizeTilesCoroutine(path, convertedQueue));
    }


    private IEnumerator VisualizeTilesCoroutine(Stack<Vector3Int> path, Queue<Vector3Int> visitedTiles)
    {
        foreach (var pos in visitedTiles)
        {
            SetTile(pos, Color.white, 0.5f);
            yield return new WaitForSeconds(visualizationDelay);
        }


        if(path != null)
        {
            foreach (var pos in path)
            {
                SetTile(pos, Color.blue, 1.0f);
                yield return new WaitForSeconds(pathDelay);
            }
        }
        else
        {
            foreach (var pos in visitedTiles)
            {
                SetTile(pos, Color.red, 0.5f);
            }
        }
    }

    public void SetTile(Vector3Int pos, Color color, float? alpha)
    {
        color.a = alpha ?? color.a;
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    public void Stop()
    {
        StopAllCoroutines();
        Clear();
    }


}
