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
        public MapResource treePrefab_2;
        public MapResource treePrefab_3;
        public MapResource treePrefab_4;
        public MapResource treePrefab_5;
        public MapResource treePrefab_6;
        public MapResource treePrefab_7;

        private MapResource ChooseTreePrefab(System.Random rndSeed)
        {
            int prefNum = rndSeed.Next(1,7);
            return prefNum switch
            {
                1 => treePrefab_1,
                2 => treePrefab_2,
                3 => treePrefab_3,
                4 => treePrefab_4,
                5 => treePrefab_5,
                6 => treePrefab_6,
                7 => treePrefab_7,
                _ => treePrefab_1,
            };
        }

        public MapResource grassPrefab_1;
        public MapResource grassPrefab_2;
        public MapResource grassPrefab_3;
        public MapResource grassPrefab_4;
        public MapResource grassPrefab_5;
        public MapResource grassPrefab_6;
        public MapResource grassPrefab_7;
        public MapResource grassPrefab_8;

        private MapResource ChooseGrassPrefab(System.Random rndSeed)
        {
            int prefNum = rndSeed.Next(1, 8);
            return prefNum switch
            {
                1 => grassPrefab_1,
                2 => grassPrefab_2,
                3 => grassPrefab_3,
                4 => grassPrefab_4,
                5 => grassPrefab_5,
                6 => grassPrefab_6,
                7 => grassPrefab_7,
                _ => grassPrefab_1,
            };
        }

        private int GetTerrainCellsCount(HexGrid grid)
        {
            int count = 0;
            foreach (var cell in grid.cellList)
                if (cell.CellColor == cell.terrainColor)
                    count++;
            return count;
        }
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
            int treeChunkCount = rndSeed.Next(GetTerrainCellsCount(grid) - 100, GetTerrainCellsCount(grid));
            while(treeChunkCount >= 0)
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
                                treeCountOnCell = rndSeed.Next(4,6);
                            for(int i = 0; i < treeCountOnCell; i++)
                            {
                                MapResource obj = Instantiate(ChooseTreePrefab(rndSeed));
                                obj.transform.SetParent(transform);
                                Vector3 pos = cell.transform.position;
                                Vector3 scale = obj.transform.localScale;

                                pos.y += obj.transform.localScale.y * 0.04f;
                                obj.transform.position = pos;

                                float scaling = UnityEngine.Random.Range(-0.5f, 0.5f);
                                scale.x += scaling;
                                scale.y += scaling;
                                scale.z += scaling;
                                obj.transform.localScale = scale;

                                obj.transform.rotation = Quaternion.Euler(-90f, UnityEngine.Random.Range(-180, 180), 0f);
                                obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));

                                treeList.Add(obj);
                                treeChunkCount--;
                            }

                        }
                    }
                }
                startCell = rndSeed.Next(grid.cellList.Length);
                while (grid.cellList[startCell].CellColor != grid.cellList[0].terrainColor)
                    startCell = rndSeed.Next(grid.cellList.Length);
            }
        }

        private void GenerateGrass(HexGrid grid, System.Random rndSeed)
        {
            foreach(var cell in grid.cellList)
                if(cell.CellColor == cell.terrainColor)
                {
                    int spawnChance = rndSeed.Next(0, 10);
                    if (spawnChance > 5)
                    {
                        int grassCount = rndSeed.Next(1, 5);
                        while(grassCount != 0)
                        {
                            MapResource obj = Instantiate(ChooseGrassPrefab(rndSeed));
                            obj.transform.SetParent(transform);
                            Vector3 pos = cell.transform.position;
                            Vector3 scale = obj.transform.localScale;

                            pos.y += obj.transform.localScale.y * 0.02f;
                            obj.transform.position = pos;

                            float scaling = UnityEngine.Random.Range(-0.3f, 0.3f);
                            scale.x += scaling;
                            scale.y += scaling;
                            scale.z += scaling;
                            obj.transform.localScale = scale;

                            obj.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-180, 180), 0f);
                            obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));

                            grassCount--;
                        }
                    }
                }
        }

        public void GenerateResource(HexGrid grid, MapResource[] resources, List<MapResource> treeList, System.Random rndSeed)
        {
            GenerateRock(grid, rndSeed);
            GenerateTree(grid, treeList, rndSeed);
            GenerateGrass(grid, rndSeed);
        }
    }
}
