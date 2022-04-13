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



        //resGen = FindObjectOfType<ResourceGenerator>();
        //resGen.GenerateResource(grid);

        /*var startUnits = FindObjectsOfType<Unit>();
        foreach(var cell in grid.cellList)
        {
            if (cell.CellType == HexCell.CellTypes.sand)
                startUnits[0].transform.localPosition = cell.transform.localPosition;
        }
        for(int i = grid.cellList.Length - 1; i >= 0; i-- )
        {
            if (grid.cellList[i].CellType == HexCell.CellTypes.sand)
                startUnits[1].transform.localPosition = grid.cellList[i].transform.localPosition;
        }*/


        //resGen.CombineMeshes();
        //GenerateWay(grid);
    }

    public void GenerateWay(HexGrid grid)
    {
        int startCell = 18;
        int endCell = 81;




        //Господи, помоги
        Vector3 startVecCoords;
        Vector3 startCoords = grid.cellList[startCell].transform.position;
        Vector3 endCoords = grid.cellList[endCell].transform.position;
        startVecCoords.x = endCoords.x - startCoords.x;
        startVecCoords.z = endCoords.z - startCoords.z;

        while (startCell != endCell)
        {
            var neighbourCells = grid.cellList.GetNeighbours(startCell);
            Vector3 nCellCoords = neighbourCells[0].transform.position;
            Vector3 neigVectorCoords;

            neigVectorCoords.x = nCellCoords.x - startCoords.x;
            neigVectorCoords.z = nCellCoords.z - startCoords.z;

            float scale = (neigVectorCoords.x * startVecCoords.x)+(neigVectorCoords.z * startVecCoords.z);
            float CosF = (scale)/((Mathf.Sqrt(Mathf.Pow(startVecCoords.x, 2) * Mathf.Pow(startVecCoords.z, 2)))*
                (Mathf.Pow(neigVectorCoords.x, 2) * Mathf.Pow(neigVectorCoords.z, 2)));



            //float globalMin = Mathf.Sqrt(Mathf.Pow(endCoords.x - nCellCoords.x, 2) + Mathf.Pow(endCoords.y - nCellCoords.y, 2) + Mathf.Pow(endCoords.z - nCellCoords.z, 2));
            foreach(var nCell in neighbourCells)
            {
                nCellCoords = nCell.transform.position;
                float min = Mathf.Sqrt(Mathf.Pow(endCoords.x - nCellCoords.x, 2) + Mathf.Pow(endCoords.y - nCellCoords.y, 2) + Mathf.Pow(endCoords.z - nCellCoords.z, 2));
                if (min < globalMin)
                {
                    globalMin = min;
                    startCell = nCell.CellIndex;
                }
            }
            grid.cellList[startCell].CellType = HexCell.CellTypes.sand;
        }

    }
}
