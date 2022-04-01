using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Assets.Map.WorldMap;

namespace Assets.Map.MapResources
{
    public class ResourceGenerator : MonoBehaviour
    {
        public MapResource rockPrefab;
        public MapResource treePrefab_1;
        private void GenerateRock(HexGrid grid, System.Random rndSeed)
        {
            foreach (var cell in grid.cellList)
                if (cell.CellColor == cell.terrainColor || cell.CellColor == cell.rockColor)
                {
                    int isRock = rndSeed.Next(1, 10);
                    var nCells = grid.cellList.GetNeighbours(cell.CellIndex);
                    bool isNearRock = false;
                    foreach (var nCell in nCells)
                        if (nCell.CellColor == nCell.rockColor)
                        {
                            isNearRock = true;
                            isRock = 10;
                        }
                    if (cell.CellColor == cell.rockColor)
                    {
                       isRock = 10;
                       isNearRock = true;
                    }
 
                    if (isRock > 7)
                    {
                        int rockCount = rndSeed.Next(1, 10);
                        if (isNearRock)
                            rockCount += 5;
                        for (int i = 0; i < rockCount; i++)
                        {
                            MapResource obj = Instantiate(rockPrefab);
                            obj.transform.SetParent(transform);
                            Vector3 pos = cell.transform.position;
                            Quaternion rotation = cell.transform.rotation;
                            Vector3 scaling = cell.transform.localScale;

                            int rotate = rndSeed.Next(-60, 60);
                            
                            float rndElevation = UnityEngine.Random.Range(0.15f, 0.35f);
                            pos.y += obj.transform.localScale.y * rndElevation;
                            obj.transform.position = pos;

                            rotation.x += rotate;
                            obj.transform.rotation = rotation;

                            scaling.x += UnityEngine.Random.Range(-1.5f, -0.5f);
                            scaling.y += UnityEngine.Random.Range(-1.5f, -0.5f);
                            scaling.z += UnityEngine.Random.Range(-1.5f, -0.5f);
                            obj.transform.localScale += scaling;

                            obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
                        }

                    }

                }
        }

        public void GenerateTree(HexGrid grid, List<MapResource> treeList, System.Random rndSeed)
        {
            int startCell = rndSeed.Next(grid.cellList.Length);
            while(grid.cellList[startCell].CellColor != grid.cellList[0].terrainColor)
                startCell = rndSeed.Next(grid.cellList.Length);
            int treeCellCount = rndSeed.Next(50, 100);
            while(treeCellCount >= 0)
            {
                if(grid.cellList[startCell].CellColor == grid.cellList[0].terrainColor)
                {
                    CellList neigboursCells = grid.cellList.GetNeighbours(startCell);
                    bool isRock = false;
                    foreach(var cell in neigboursCells)
                        if(cell.CellColor == cell.rockColor)
                            isRock = true;
                    foreach(var cell in neigboursCells)
                    {
                        if(cell.CellColor == cell.terrainColor)
                        {
                            int treeCountOnCell = 0;
                            if (isRock)
                                treeCountOnCell = rndSeed.Next(0,1);
                            else
                                treeCountOnCell = rndSeed.Next(2,4);
                            for(int i = 0; i < treeCountOnCell; i++)
                            {
                                MapResource obj = Instantiate(treePrefab_1);
                                obj.transform.SetParent(transform);
                                Vector3 pos = cell.transform.position;
                                Vector3 scale = obj.transform.localScale;

                                pos.y += obj.transform.localScale.y * 0.04f;
                                obj.transform.position = pos;

                                int scaling = rndSeed.Next(-10, 10);
                                scale.x += scaling;
                                scale.y += scaling;
                                scale.z += scaling;
                                obj.transform.localScale = scale;

                                obj.transform.rotation = Quaternion.Euler(-90f, UnityEngine.Random.Range(-180, 180), 0f);
                                obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));

                                treeList.Add(obj);
                                treeCellCount--;
                            }

                        }
                    }
                }
                startCell = rndSeed.Next(grid.cellList.Length);
                while (grid.cellList[startCell].CellColor != grid.cellList[0].terrainColor)
                    startCell = rndSeed.Next(grid.cellList.Length);
            }
        }

        public void GenerateResource(HexGrid grid, MapResource[] resources, List<MapResource> treeList, System.Random rndSeed)
        {
            GenerateRock(grid, rndSeed);
            GenerateTree(grid, treeList, rndSeed);
        }
    }
}
