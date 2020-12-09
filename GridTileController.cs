using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileController : MonoBehaviour {

    public GameObject tilePrefab;
    public GameObject completeFloor;
    public GridTile[,] tiles;
    int size;

    List<GridTile> highlighted = new List<GridTile>();
    GameObject tileHolder;
    GridTile selected;
    public GridTile Selected
    {
        get { return selected; }
        set
        {
            //selected.
        }
    }
    

    public void BuildGridTiles(int gridSize)
    {
        if (completeFloor != null) completeFloor.SetActive(false);
        if (tileHolder != null) Destroy(tileHolder);
        tileHolder = new GameObject("Tiles");
        tileHolder.transform.SetParent(transform);
        tiles = new GridTile[gridSize, gridSize];

        size = gridSize;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform.position + new Vector3(x, 0, y), Quaternion.identity);
                newTile.name = "Tile (" + x + "," + y + ")";
                tiles[x, y] = newTile.GetComponent<GridTile>();
                tiles[x, y].position = new GridPoint(x, y);
                tiles[x, y].SetNormal();
                newTile.transform.SetParent(tileHolder.transform);
            }
        }
    }

    public void ClearHighlighted()
    {
        for (int i = 0; i < highlighted.Count; i++)
        {
            highlighted[i].SetNormal();
        }
        highlighted.Clear();
    }

    public void HighlightMoveableOnNextTurn()
    {
        int[,] moveMap = TurnManager.currentTurn.combatant.gridAgent.MoveMap;
        int gridSize = moveMap.GetLength(0);
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {

                if (moveMap[x, y] > -1)
                {
                    tiles[x, y].SetMoveable();
                    highlighted.Add(tiles[x, y]);
                }
                else tiles[x, y].SetNormal();
            }
        }
    }

    public void HighlightAttackable(GridPoint[] attackable)
    {
        ClearHighlighted();
        for (int i = 0; i < attackable.Length; i++)
        {
            tiles[attackable[i].x, attackable[i].y].SetAttackable();
            highlighted.Add(tiles[attackable[i].x, attackable[i].y]);
        }
    }

    public void HighlightMoveableTiles(int[,] moveMap)
    {
        int gridSize = moveMap.GetLength(0);
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {

                if(moveMap[x,y] > -1)
                {
                    tiles[x,y].SetMoveable();
                    highlighted.Add(tiles[x, y]);
                }
                else tiles[x, y].SetNormal();
            }
        }
    }
}
