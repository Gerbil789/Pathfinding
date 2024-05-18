using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

public class BidirectionalAstar : Pathfinding
{
    public override Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
    {
        return new Stack<Vector3Int>();
        //CancellationTokenSource cts = new CancellationTokenSource();
        //Task<Stack<Vector3Int>> task1 = Task.Run(() => Astar.GetPath(start, end, cts.Token));
        //Task<Stack<Vector3Int>> task2 = Task.Run(() => Astar.GetPath(end, start, cts.Token));

        //Task<Stack<Vector3Int>> completedTask = await Task.WhenAny(task1, task2);

        //cts.Cancel();  // Request cancellation of the other task

        //try
        //{
        //    return await completedTask;
        //}
        //catch (OperationCanceledException)
        //{
        //    return null;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.Message);
        //    return null;
        //}
    }
}

