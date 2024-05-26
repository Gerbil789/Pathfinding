using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using System.Runtime.CompilerServices;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.Jobs;
using Unity.VisualScripting;
using Unity.Burst;

public class DOTS_Astar : Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public struct Node
    {
        public int index;
        public int parentIndex;

        public int x;
        public int y;


        public int G;
        public int H;
        public int F;

        public void CalculateF() => F = G + H;
    }



    private int2 gridSize = new int2(MapManager.MapSize.x, MapManager.MapSize.y);




    public override Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        visitedTiles = new(); //for visualization
        if (!IsValidInput(start, end)) return null;
        path = null;

        NativeArray<Node> pathNodeArray = new NativeArray<Node>(MapManager.MapSize.x * MapManager.MapSize.y, Allocator.TempJob);
        NativeQueue<int2> visitedNodes = new NativeQueue<int2>(Allocator.TempJob);

        FindPathJob job = new FindPathJob
        {
            startPosition = new int2(start.x, start.y),
            endPosition = new int2(end.x, end.y),
            pathNodeArray = pathNodeArray,
            visitedNodes = visitedNodes
        };
        JobHandle handle = job.Schedule();
        handle.Complete();

        var endNode = pathNodeArray[job.CalculateIndex(end.x, end.y)];

        if (endNode.parentIndex != -1) {
            path = GeneratePath(job.pathNodeArray, endNode);
        }

        while (visitedNodes.Count > 0)
        {
            int2 node = visitedNodes.Dequeue();
            visitedTiles.Enqueue(new Vector2Int(node.x, node.y));
        }

        pathNodeArray.Dispose();
        visitedNodes.Dispose();
        return path;
    }

    [BurstCompile]
    public struct FindPathJob : IJob
    {
        public int2 startPosition;
        public int2 endPosition;


        public NativeArray<Node> pathNodeArray;
        public NativeQueue<int2> visitedNodes;


        public void Execute()
        {
            //initialize
            for (int x = 0; x < MapManager.MapSize.x; x++)
            {
                for (int y = 0; y < MapManager.MapSize.y; y++)
                {
                    if (!MapManager.Map[x, y]) continue; //skip unwalkable tiles

                    Node node = new();
                    node.x = x;
                    node.y = y;
                    node.index = CalculateIndex(x, y);
                    node.parentIndex = -1;
                    node.G = int.MaxValue;
                    node.H = CalculateDistanceCost(new int2(x, y), endPosition);
                    node.CalculateF();

                    pathNodeArray[node.index] = node;
                }
            }

            NativeArray<int2> neighborOffsetArray = new NativeArray<int2>(8, Allocator.Temp);

            neighborOffsetArray[0] = new int2(-1, 0); //left
            neighborOffsetArray[1] = new int2(+1, 0); //right
            neighborOffsetArray[2] = new int2(0, +1); //up
            neighborOffsetArray[3] = new int2(0, -1); //down
            neighborOffsetArray[4] = new int2(-1, -1); //down left
            neighborOffsetArray[5] = new int2(-1, +1); //up left
            neighborOffsetArray[6] = new int2(+1, -1); //down right
            neighborOffsetArray[7] = new int2(+1, +1); //up right
            

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            //run algorithm
            Algorithm(openList, closedList, neighborOffsetArray);

            //manually dispose of native arrays
            neighborOffsetArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        public int CalculateIndex(int x, int y)
        {
            return x + y * MapManager.MapSize.x;
        }


        private int CalculateDistanceCost(int2 a, int2 b)
        {
            int x = math.abs(a.x - b.x);
            int y = math.abs(a.y - b.y);
            int remaining = math.abs(x - y);
            return MOVE_DIAGONAL_COST * math.min(x, y) + MOVE_STRAIGHT_COST * remaining;
        }


        void Algorithm(NativeList<int> openList, NativeList<int> closedList, NativeArray<int2> neighborOffsetArray)
        {
            int startNodeIndex = CalculateIndex(startPosition.x, startPosition.y);
            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y);

            Node startNode = pathNodeArray[startNodeIndex];
            startNode.G = 0;
            startNode.CalculateF();
            pathNodeArray[startNode.index] = startNode;

            openList.Add(startNodeIndex);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList);
                Node currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    //reached the end -> generate path and return
                    //Node endNode = currentNode;
                    //while (endNode.index != -1)
                    //{
                    //    visitedTiles.Enqueue(new Vector2Int(endNode.x, endNode.y));
                    //    endNode = pathNodeArray[endNode.parentIndex];
                    //}
                    //path = new Stack<Vector3Int>(visitedTiles.Select(x => (Vector3Int)x));
                    break;
                }

                //remove current node from open list and add to closed list
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }
                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighborOffsetArray.Length; i++)
                {
                    int2 neighborOffset = neighborOffsetArray[i];
                    int2 neighborPosition = new int2(currentNode.x + neighborOffset.x, currentNode.y + neighborOffset.y);

                    if(IsOutOfBounds(neighborPosition)) continue; //skip if out of bounds
                    if (!MapManager.Map[neighborPosition.x, neighborPosition.y]) continue; //skip unwalkable tiles

                    int neighborNodeIndex = CalculateIndex(neighborPosition.x, neighborPosition.y);
                    if (closedList.Contains(neighborNodeIndex)) continue; //skip if already in closed list

                    Node neighborNode = pathNodeArray[neighborNodeIndex];

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                    int tentativeG = currentNode.G + CalculateDistanceCost(currentNodePosition, neighborPosition);
                    if (tentativeG < neighborNode.G)
                    {
                        neighborNode.parentIndex = currentNodeIndex;
                        neighborNode.G = tentativeG;
                        neighborNode.CalculateF();
                        pathNodeArray[neighborNodeIndex] = neighborNode;

                        if (!openList.Contains(neighborNodeIndex))
                        {
                            openList.Add(neighborNodeIndex);
                            visitedNodes.Enqueue(new int2(neighborPosition.x, neighborPosition.y));
                        }
                    }
                }




            }

            Node endNode = pathNodeArray[endNodeIndex];
            if (endNode.parentIndex == -1) return; //no path found








        }

        private int GetLowestCostFNodeIndex(NativeList<int> openList)
        {
            Node lowestCostFNode = pathNodeArray[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                Node testNode = pathNodeArray[openList[i]];
                if (testNode.F < lowestCostFNode.F)
                {
                    lowestCostFNode = testNode;
                }
            }
            return lowestCostFNode.index;
        }

        bool IsOutOfBounds(int2 pos)
        {
            return pos.x < 0 || pos.y < 0 || pos.x >= MapManager.MapSize.x || pos.y >= MapManager.MapSize.y;
        }


    }

    Stack<Vector3Int> GeneratePath(NativeArray<Node> nodes, Node current)
    {
        Stack<Vector3Int> path = new();

        while (current.parentIndex != -1)
        {
            path.Push(new Vector3Int(current.x, current.y, 0));
            current = nodes[current.parentIndex];
        }
        return path;
    }



}

