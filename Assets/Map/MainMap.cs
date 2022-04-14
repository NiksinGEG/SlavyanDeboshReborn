using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Map.MapResources;

public class MainMap : MonoBehaviour
{
    public HexGrid grid;
    public ResourceGenerator resGen;

    private void Awake()
    {
        UnityEngine.Random.InitState(GlobalVariables.Seed);
        grid = Instantiate(grid);
        grid.transform.position = Vector3.zero;
        grid.transform.SetParent(transform);



        resGen = FindObjectOfType<ResourceGenerator>();
        resGen.GenerateResource(grid);

        var startUnits = FindObjectsOfType<Unit>();
        foreach(var cell in grid.cellList)
        {
            if (cell.CellType == HexCell.CellTypes.sand)
                startUnits[0].transform.localPosition = cell.transform.localPosition;
        }
        for(int i = grid.cellList.Length - 1; i >= 0; i-- )
        {
            if (grid.cellList[i].CellType == HexCell.CellTypes.sand)
                startUnits[1].transform.localPosition = grid.cellList[i].transform.localPosition;
        }


        //resGen.CombineMeshes();
        //GenerateWay(grid);
    }

    public void GenerateWay(HexGrid grid)
    {
        int startCell = 18;
        int endCell = 81;


    }
}
