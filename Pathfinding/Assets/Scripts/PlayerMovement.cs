using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    private Stack<Vector3Int> path;

    private Coroutine movementCoroutine;

    void Start()
    {
        transform.position = new Vector3(MapManager.MapSize.x / 2, MapManager.MapSize.y / 2, 0);
    }

    void Update()
    {
        if (path != null && path.Count > 0 && movementCoroutine == null)
        {
            Vector3Int target = path.Pop();

            //use coroutine to move smoothly independent of frame rate
            movementCoroutine = StartCoroutine(MoveToTarget(target));
        }
    }

    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        movementCoroutine = null; // Reset coroutine to allow movement to the next target
    }

    public void SetPath(Stack<Vector3Int> _path)
    {
        path = _path;
    }
}
