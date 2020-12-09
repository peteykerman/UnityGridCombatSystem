using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAgent : MonoBehaviour {

    public GridPoint position;
    public int agentID;
    public int movementPoints = 10;
    public float moveSpeed = 5f;
    public bool moving;

    public int[,] MoveMap { get { return GridMaster.grid.MoveMap(position, movementPoints); } }




    // Use this for initialization
    void Start () {
        transform.position = position.ToVector3();
    }
    private void OnEnable()
    {
        GridMaster.agents.Add(this);
        agentID = GridMaster.agents.IndexOf(this);
    }
    private void OnDisable()
    {
        GridMaster.agents.Remove(this);
    }
    

    public bool MoveTo(GridPoint newPosition, out Coroutine moveRoutine)
    {
        if (MoveMap[newPosition.x, newPosition.y] > 0)
        {
            StopAllCoroutines();
            moveRoutine = StartCoroutine(Move(GridPathfinder.GetMoveQueue(MoveMap, position, newPosition)));
            GridMaster.grid.OnAgentMove(position, newPosition, agentID);
            return true;
        }
        else
        {
            moveRoutine = null;
            return false;
        }
    }
    IEnumerator Move(Queue<GridPoint> points)
    {
        if (points.Count == 0) yield break;
        GridPoint current = position;
        int count = points.Count;
        moving = true;
        while(count > 0)
        {
            GridPoint next = points.Dequeue();
            count--;
            float timer = 0;
            while (timer < 1f)
            {
                timer += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(current.ToVector3(), next.ToVector3(), timer);
                yield return null;
            }
            current = next;
            position = current;
            yield return null;
        }
        moving = false;
    }
    public void SetPositionIndex()
    {
        GridMaster.grid.gridIndex[position.x, position.y] = agentID;
    }
    public void SetPositionIndex(int[,] map)
    {
        map[position.x, position.y] = agentID;
    }
    public GridPoint[] GetMoveable()
    {
        List<GridPoint> moveable = new List<GridPoint>();
        int[,] move = MoveMap;
        for (int x = 0; x < move.GetLength(0); x++)
        {
            for (int y = 0; y < move.GetLength(1); y++)
            {
                if(move[x,y] > -1)
                {
                    moveable.Add(new GridPoint(x, y));
                }
            }
        }
        return moveable.ToArray();
    }

    private void OnValidate()
    {
        //position = new GridPoint((int)transform.position.x, (int)transform.position.z);
        transform.position = position.ToVector3();
    }
}
