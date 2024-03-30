using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

public static partial class Pathfinding
{
    public static class BidirectionalAstar
    {
        public static Stack<Vector3Int> GetPath(Vector3Int start, Vector3Int end)
        {
            try
            {
                path = null;

                //Thread thread1 = new Thread(() => GetPath(start, end));
                //Thread thread2 = new Thread(() => GetPath(end, start));

                //thread1.Start();
                //thread2.Start();



              

                return path;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }
    }
}
