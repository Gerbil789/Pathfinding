using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Astar : MonoBehaviour
{
    [SerializeField] Tilemap tileMap;
    [SerializeField] Vector3Int startPos, goalPos;
    [HideInInspector] public List<Vector3Int> unwalkableTiles = new List<Vector3Int>();
    Stack<Vector3Int> path;
     HashSet<Node> openList;
     HashSet<Node> closedList;
     Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();
     private Node current;
    
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.Tab)){
            AstarDebugger.MyInstance.ShowHide();
        }  
    }

    void Initialize()
    {
        current = GetNode(startPos);

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();
        openList.Add(current);
    }

    void Algorithm(){
        if(current == null){
            Initialize();
        }

        while(openList.Count > 0 && path == null){
            List<Node> neighbors = FindNeighbors(current.position);
            ExamineNeighbors(neighbors, current);
            UpdateCurrentTile( ref current);

            path = GeneratePath(current);
        }

        
        AstarDebugger.MyInstance.CreateTiles(openList, closedList, startPos, goalPos, path);
    }

    List <Node> FindNeighbors(Vector3Int parentPos){
        List<Node> neighbors = new List<Node>();
        for(int x = -1 ; x <= 1; x++){
            for(int y = -1 ; y <= 1; y++){
                Vector3Int neighborPos = new Vector3Int(parentPos.x + x, parentPos.y + y, parentPos.z);
                if(x !=0 || y !=0){ 
                    if(neighborPos != startPos && tileMap.GetTile(neighborPos) && !unwalkableTiles.Contains(neighborPos)){
                        neighbors.Add(GetNode(neighborPos));
                    }   
                }
            }
        }
        return neighbors;
    }

    void ExamineNeighbors(List<Node> neighbors, Node current){
        for(int i = 0; i < neighbors.Count; i++){
            Node neighbor = neighbors[i];

            if(!ConnectedDiagonally(current, neighbor)){
                continue;
            }

            int gScore = DetermineGScore(neighbors[i].position, current.position);

            if(openList.Contains(neighbor)){
                if(current.G + gScore < neighbor.G){
                    CalcValues(current, neighbor, gScore);
                }
            }else if(!closedList.Contains(neighbor)){
                CalcValues(current, neighbor, gScore);
                openList.Add(neighbor);
            }
        }
    }

    void CalcValues(Node parent, Node neighbor, int cost){
        neighbor.parent = parent;
        neighbor.G = parent.G + cost;
        neighbor.H = ((Mathf.Abs((neighbor.position.x - goalPos.x)) + (Mathf.Abs(neighbor.position.y - goalPos.y))) * 10);
        neighbor.F = neighbor.G + neighbor.H;
    }

    int DetermineGScore(Vector3Int neighbor, Vector3Int current){
        int gScore = 0;
        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;
        if(Mathf.Abs(x-y) % 2 == 1){
            gScore = 10;
        }else{
            gScore = 14;
        }
        return gScore;
    }

    void UpdateCurrentTile(ref Node current){
        openList.Remove(current);
        closedList.Add(current);

        if(openList.Count > 0){
            current = openList.OrderBy(x => x.F).First();
        }

    }

    Node GetNode(Vector3Int pos)
    {
        if(allNodes.ContainsKey(pos))
        {
            return allNodes[pos];
        }
        else
        {
            Node node = new Node(pos);
            allNodes.Add(pos, node);
            return node;
        }
    }

    bool ConnectedDiagonally(Node currentNode, Node neighbor){
        Vector3Int dir = currentNode.position - neighbor.position;
        Vector3Int first = new Vector3Int(currentNode.position.x + (dir.x * -1), currentNode.position.y, currentNode.position.z);
        Vector3Int second = new Vector3Int(currentNode.position.x, currentNode.position.y + (dir.y * -1), currentNode.position.z);

        if(unwalkableTiles.Contains(first) || unwalkableTiles.Contains(second)){
            return false;
        }
        return true;
    }
    Stack<Vector3Int> GeneratePath(Node current){
        if(current.position == goalPos){
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            while(current.position != startPos){
                finalPath.Push(current.position);
                current = current.parent;
            }
            return finalPath;
        }
        return null;
    }
    void clearTiles(){
        AstarDebugger.MyInstance.ClearTiles(allNodes);
        allNodes.Clear();
        path = null;
        current = null;
    }

    public Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int goal){
        
        startPos = start;
        if(unwalkableTiles.Contains(goal)) return null;
        goalPos = goal;
        clearTiles();
        Algorithm();
        return path;
    }
}
