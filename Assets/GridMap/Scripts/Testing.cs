using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTools;

public class Testing : MonoBehaviour
{
    private Grid grid;
    
    private void Start()
    {
        grid = new Grid(5, 5, 1);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.FillCell(Tools.GetMouseWordPosition());
        }
        else if (Input.GetMouseButtonDown(1))
        {
            grid.UnFillCell(Tools.GetMouseWordPosition());
        }
    }
}
