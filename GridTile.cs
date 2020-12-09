using UnityEngine;
using System.Collections;

public class GridTile : MonoBehaviour
{
    public GridPoint position;
    public Color normal = Color.gray;
    public Color moveable = Color.blue;
    public Color attackable = Color.red;
    public Color selected = Color.yellow;

    public Vector3 WorldPos { get { return transform.position; } }

    public void SetMoveable()
    {
        gameObject.GetComponentInChildren<Renderer>().material.color = moveable;
        
        
    }
    public void SetNormal()
    {
        gameObject.GetComponentInChildren<Renderer>().material.color = normal;
    }
    public void SetAttackable()
    {
        gameObject.GetComponentInChildren<Renderer>().material.color = attackable;
    }
    public void SetSelected()
    {
        gameObject.GetComponentInChildren<Renderer>().material.color = selected;
    }
}
