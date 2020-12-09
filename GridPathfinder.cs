using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GridPathfinder
{
    public struct Node
    {
        public GridPoint position;
        public int steps;

        public Node(GridPoint _position, int _steps)
        {
            position = _position;
            steps = _steps;
        }
    }
    
    public static Queue<GridPoint> GetMoveQueue(int[,] movementMap, GridPoint startPoint, GridPoint endPoint)
    {
        
        List<GridPoint> pathList = new List<GridPoint>();
        GridPoint current = endPoint;
        
        while(current != startPoint)
        {            
            pathList.Add(current);
            int x = current.x;
            int y = current.y;
            bool pathFound = false;
            for (int u = x - 1; u <= x + 1; u++)
            {
                for (int v = y - 1; v <= y + 1; v++)
                {                    
                    if ((u == x || v == y) && InsideMap(movementMap, u, v))
                    {
                        if(movementMap[u,v] == movementMap[x, y] - 1)
                        {
                            if (pathFound && Random.value > .5f) continue;
                            current = new GridPoint(u, v);
                            pathFound = true;
                        }
                    }
                }
            }
            if(!pathFound) return new Queue<GridPoint>();
        }
        
        Queue<GridPoint> pathQueue = new Queue<GridPoint>();
        for (int i = pathList.Count - 1; i >= 0; i--)
        {
            pathQueue.Enqueue(pathList[i]);
        }
        return pathQueue;
    }
    public static int[,] MoveabilityMap(bool[,] obstacleMap, GridPoint startPoint, int maxSteps)
    {
        int[,] moveMap = BlankMoveMap(obstacleMap.GetLength(1));
        
        if (!startPoint.IsInsideMap(moveMap)) return moveMap;
        bool[,] checkedMap = obstacleMap.Clone() as bool[,]; 

        Node start = new Node(startPoint, 0);
        Queue<Node> q = new Queue<Node>();
        int x;
        int y;
        int count = 1;
        q.Enqueue(start);
        

        while (count > 0)
        {
            Node checking = q.Dequeue();
            count--;
            x = checking.position.x;
            y = checking.position.y;
            moveMap[x, y] = checking.steps;
            checkedMap[x, y] = true;
            if (checking.steps == maxSteps) continue;

            for (int u = x-1 ; u <= x+1; u++)
            {
                for (int v = y-1; v <= y+1; v++)
                {
                    if ((u == x || v == y) && InsideMap(checkedMap, u, v))
                    {
                        if(checkedMap[u,v] == false)
                        {
                            Node newNode = new Node(new GridPoint(u, v), checking.steps + 1);
                            q.Enqueue(newNode);
                            count++;
                            checkedMap[u, v] = true;
                        }
                    }
                }
            }
        }

        return moveMap;
    }

    public static bool InsideMap(bool[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }
    public static bool InsideMap(int[,] map, int x, int y)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }
    static int[,] BlankMoveMap(int size)
    {
        int[,] blank = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                blank[x, y] = -1;
            }
        }
        return blank;
    }
}
