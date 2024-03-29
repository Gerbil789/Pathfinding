using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AlgorithmVisualizer : MonoBehaviour
{
    static AlgorithmVisualizer instance;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private float visualizationDelay = 0.05f;

    public static event Action VisualizationFinished;

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

    private Queue<VisualizationData> tilesQueue = new Queue<VisualizationData>();
    private Queue<VisualizationData> pathQueue = new Queue<VisualizationData>();

    private bool isVisualizing = false;
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

    public void VisualizeTile(Vector2Int position)
    {
        tilesQueue.Enqueue(new VisualizationData((Vector3Int)position, Color.white, 0.1f));
        if (!isVisualizing)
        {
            StartCoroutine(VisualizeTilesCoroutine());
        }
    }

    private IEnumerator VisualizeTilesCoroutine()
    {
        isVisualizing = true;

        while (tilesQueue.Count > 0)
        {
            var data = tilesQueue.Dequeue();

            SetTile(data.position, data.color, null);

            yield return new WaitForSeconds(visualizationDelay);
        }

        while (pathQueue.Count > 0)
        {
            VisualizationData data = pathQueue.Dequeue();
            SetTile(data.position, data.color, null);
            yield return new WaitForSeconds(visualizationDelay);
        }

        isVisualizing = false;
        VisualizationFinished?.Invoke();
    }


    public void VisualizePath(Stack<Vector3Int> path)
    {
        foreach (var pos in path)
        {
            pathQueue.Enqueue(new VisualizationData(pos, Color.blue, 0.3f));
        }

        if (!isVisualizing)
        {
            StartCoroutine(VisualizeTilesCoroutine());
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
}
