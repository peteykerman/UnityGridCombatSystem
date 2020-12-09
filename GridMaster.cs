using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaster : MonoBehaviour {

    static GridMaster gm;
    public static GridMaster GM
    {
        get
        {
            if (gm != null) return gm;
            else return gm = FindObjectOfType<GridMaster>();
        }
    }


    public static List<GridObstacle> obstacles = new List<GridObstacle>();
    public static List<GridAgent> agents = new List<GridAgent>();

    public GridTileController tileController;

    public static Grid grid;
    public int gridSize;

    

	// Use this for initialization
	void Awake () {
        gm = this;
        //Debug.Log(agents.Count);
        //grid = new Grid(gridSize, agents);
        tileController = GetComponentInChildren<GridTileController>();
        tileController.BuildGridTiles(gridSize);
    }
    private void Start()
    {
        grid = new Grid(gridSize, agents);
    }

    // Update is called once per frame
    void Update () {
        
    }
    
    public bool[,] GetObstacleMap()
    {
        bool[,] map = new bool[gridSize, gridSize];
        
        int agentCount = agents.Count;
        int obstacleCount = obstacles.Count;
        
        for (int i = 0; i < agentCount; i++)
        {
            agents[i].position.SetOccupied(map);
        }
        for (int i = 0; i < obstacleCount; i++)
        {
            obstacles[i].position.SetOccupied(map);
        }
        return map;        
    }
    
    
      
}

