using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GridObj grid;
    private void Start()
    {
        grid.CreateWall(transform.position - transform.localScale / 2, transform.position + transform.localScale / 2);
    }
}
