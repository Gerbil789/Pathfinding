using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    void Start()
    {
        transform.position = new Vector3(MapManager.MapSize.x / 2, MapManager.MapSize.y / 2, 0);
        InputManager.PathCalculated += Move;
    }

    public void Move(Stack<Vector3Int> path)
    {
        Stack<Vector3> path3 = new();
        foreach (var item in path)
        {
            path3.Push(item);
        }

        StartCoroutine(MoveCoroutine(path3));
    }

    private IEnumerator MoveCoroutine(Stack<Vector3> path)
    {
        foreach (var tilePosition in path)
        {
            while (Vector3.Distance(transform.position, tilePosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, tilePosition, Time.deltaTime * speed);
                yield return null;
            }
            transform.position = tilePosition;
        }
    }
}
