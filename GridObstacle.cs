using UnityEngine;
using System.Collections;

public class GridObstacle : MonoBehaviour
{

    public GridPoint position;

    // Use this for initialization
    void Start()
    {
        transform.position = position.ToVector3();
    }
    private void OnEnable()
    {
        GridMaster.obstacles.Add(this);
    }
    private void OnDisable()
    {
        GridMaster.obstacles.Remove(this);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
