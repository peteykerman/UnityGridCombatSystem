using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public int gridSize;
    public int[,] gridIndex;
    public bool[,] obstacleMap;

    #region Constructors
    public Grid(int size)
    {
        gridSize = size;
        obstacleMap = new bool[size, size];
        gridIndex = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                gridIndex[x, y] = -1;
            }
        }
    }
    public Grid(int size, List<GridAgent> agents)
    {
        gridSize = size;
        obstacleMap = new bool[size, size];
        gridIndex = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                gridIndex[x, y] = -1;
            }
        }
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].SetPositionIndex(gridIndex);
            agents[i].position.SetOccupied(obstacleMap);
        }
    }
    #endregion

    public int[] GridIndexes(GridPoint[] points)
    {

        int[] toReturn = new int[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            toReturn[i] = GridIndex(points[i]);
        }
        return toReturn;
    }
    public int GridIndex(GridPoint point)
    {
        if (!point.IsInsideMap(gridIndex)) return -1;
        return gridIndex[point.x, point.y];
    }
    public int[,] MoveMap(GridPoint startPoint, int maxSteps)
    {
        int[,] moveMap = GridPathfinder.MoveabilityMap(obstacleMap, startPoint, maxSteps);
        return moveMap;
    }

    public bool AttemptMove(GridPoint start, GridPoint end, int[,] moveMap)
    {
        Queue<GridPoint> path = GridPathfinder.GetMoveQueue(moveMap, start, end);
        if (path.Count > 0) return true;
        return false;
    }
    public Queue<GridPoint> GetPath(GridPoint start, GridPoint end, int[,] moveMap)
    {
        Queue<GridPoint> path = GridPathfinder.GetMoveQueue(moveMap, start, end);
        return path;
    }
    public Queue<GridPoint> GetPath(GridPoint start, GridPoint end)
    {
        int[,] moveMap = GridPathfinder.MoveabilityMap(obstacleMap, start, gridSize * gridSize);
        Queue<GridPoint> path = GridPathfinder.GetMoveQueue(moveMap, start, end);
        return path;
    }
    public void OnAgentMove(GridPoint oldPosition, GridPoint newPosition, int agentID)
    {
        gridIndex[oldPosition.x, oldPosition.y] = -1;
        obstacleMap[oldPosition.x, oldPosition.y] = false;

        gridIndex[newPosition.x, newPosition.y] = agentID;
        obstacleMap[newPosition.x, newPosition.y] = true;
    }
    public GridPoint[] CropToMap(GridPoint[] toCrop)
    {
        List<GridPoint> cropped = new List<GridPoint>(toCrop);
        int count = cropped.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (!cropped[i].IsInsideMap(gridIndex))
            {
                cropped.RemoveAt(i);
            }
        }
        return cropped.ToArray();
    }
    public GridPoint[] CropToEmpty(GridPoint[] toCrop)
    {
        List<GridPoint> cropped = new List<GridPoint>(toCrop);
        int count = cropped.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (!cropped[i].IsInsideMap(gridIndex))
            {
                cropped.RemoveAt(i);
                continue;
            }
            if(GridIndex(cropped[i]) != -1)
            {
                cropped.RemoveAt(i);
            }
        }
        return cropped.ToArray();
    }
}
[System.Serializable]
public struct GridPoint
{
    public int x;
    public int y;

    public GridPoint(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public static GridPoint operator -(GridPoint a, GridPoint b)
    {
        a.x -= b.x;
        a.y -= b.y;
        return a;
    }
    public static GridPoint operator *(GridPoint a, int i)
    {
        a.x *= i;
        a.y *= i;
        return a;
    }
    public static GridPoint operator +(GridPoint a, GridPoint b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }
    public override bool Equals(object obj)
    {
        GridPoint test = (GridPoint)obj;
        return test == this;
    }
    public override int GetHashCode()
    {
        return (1000 * x) + y;
    }
    public static bool operator ==(GridPoint a, GridPoint b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(GridPoint a, GridPoint b)
    {
        return a.x != b.x || a.y != b.y;
    }
    public bool IsInsideMap(bool[,] map)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }
    public bool IsInsideMap(int[,] map)
    {
        return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
    }
    public bool IsInsideMap(int size)
    {
        return x >= 0 && y >= 0 && x < size && y < size;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, 0, y);
    }
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }
    public int DistanceTo(GridPoint b)
    {
        GridPoint a = this;
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);
        return x + y;
    }
    public void ClampToOne()
    {
        x = Mathf.Clamp(x, -1, 1);
        y = Mathf.Clamp(y, -1, 1);
    }
    public void SetOccupied(bool[,] map)
    {
        map[x, y] = true;
    }
    public GridPoint[] ExtendedToSize(int size)
    {
        List<GridPoint> rangeList = new List<GridPoint>((size * 4) + 1);

        for (int x = -size; x <= size; x++)
        {
            int sizeMinX = size - Mathf.Abs(x);
            for (int y = -sizeMinX; y <= sizeMinX; y++)
            {
                rangeList.Add(new GridPoint(x,y));
            }
        }

        GridPoint[] toReturn = rangeList.ToArray();
        for (int i = 0; i < toReturn.Length; i++)
        {
            toReturn[i] += this;
        }

        return toReturn;
    }
    public GridPoint[] GetCorners()
    {
        GridPoint[] corners = new GridPoint[4];
        corners[0] = this + SouthWest;
        corners[1] = this + NorthWest;
        corners[2] = this + SouthEast;
        corners[3] = this + NorthEast;
        return corners;
    }
    public GridPoint[] GetCardinals()
    {
        GridPoint[] sides = new GridPoint[4];
        sides[0] = this + West;
        sides[1] = this + South;
        sides[2] = this + North;
        sides[3] = this + East;
        return sides;        
    }
    
    public static GridPoint West { get { return new GridPoint(-1, 0); } }
    public static GridPoint East { get { return new GridPoint(1, 0); } }
    public static GridPoint North { get { return new GridPoint(0, 1); } }
    public static GridPoint South { get { return new GridPoint(0, -1); } }
    public static GridPoint NorthWest { get { return new GridPoint(-1, 1); } }
    public static GridPoint NorthEast { get { return new GridPoint(1, 1); } }
    public static GridPoint SouthWest { get { return new GridPoint(-1, -1); } }
    public static GridPoint SouthEast { get { return new GridPoint(1, -1); } }
    public static GridPoint[] Corners { get { return new GridPoint[] { NorthEast, NorthWest, SouthEast, SouthWest }; } }
    public static GridPoint[] NorthCorners { get { return new GridPoint[] { NorthEast, NorthWest }; } }
    public static GridPoint[] SouthCorners { get { return new GridPoint[] { SouthEast, SouthWest }; } }
    public static GridPoint[] Cardinals { get { return new GridPoint[] { North, West, East, South }; } }

    public static GridPoint[] RangeCluster(int range)
    {
        List<GridPoint> toReturn = new List<GridPoint>((range * 4) + 1);

        for (int x = -range; x <= range; x++)
        {
            int sizeMinX = range - Mathf.Abs(x);
            for (int y = -sizeMinX; y <= sizeMinX; y++)
            {
                toReturn.Add(new GridPoint(x, y));
            }
        }
        return toReturn.ToArray();
    }
}