using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stack<Vector3Int> path;
    [SerializeField] private float movementSpeed = 1f;
    private Vector3 dirNormalized = Vector3.zero;
    private Vector3 target;

    void Start()
    {
        transform.position = new Vector3(MapManager.MapSize.x / 2, MapManager.MapSize.y / 2, 0);
    }

    void Update()
    {
        if(path != null)
        {
            if(path.Count > 0)
            {
                target = path.Peek();
            
                dirNormalized = (target - transform.position).normalized;
            
                if(Vector3.Distance(target, transform.position) <= 0.01f){
                    target = path.Pop();  
                }else{
                    transform.Translate(dirNormalized * Time.deltaTime * movementSpeed, Space.Self);
                }
            }   
        }      
        
    }

    public void SetPath(Stack<Vector3Int> _path)
    {
        path = _path;
    }
}
