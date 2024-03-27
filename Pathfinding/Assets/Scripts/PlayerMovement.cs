using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    Astar astar;
    Camera cam;
    [SerializeField] Tilemap tileMap;
    [SerializeField] LayerMask layer;
    public Vector2Int startPos;
    [SerializeField]Vector3Int start, goal;
    Stack<Vector3Int> path;
    [SerializeField] float movementSpeed = 1f;
    Vector3 dirNormalized = Vector3.zero;
    Vector3 target;

    void Start()
    {
        astar = FindObjectOfType<Astar>();
        cam = FindObjectOfType<Camera>();
        transform.position = new Vector3(startPos.x, startPos.y, 0);
        //InvokeRepeating("Move", 0.5f, 0.5f);  
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layer); 
            if(hit.collider != null)
            {
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int clickPos = tileMap.WorldToCell(mouseWorldPos);

                goal = clickPos;
                start = tileMap.WorldToCell(transform.position);
                if(start == goal) return;
                path = astar.GetPath(start, goal);
            }
        }  
        if(path != null){
            if(path.Count > 0){
            
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
}
