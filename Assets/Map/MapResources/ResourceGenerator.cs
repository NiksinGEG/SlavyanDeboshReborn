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
        PrefabManager manager;
        [SerializeField] List<MapResource> grassList;
        [SerializeField] List<MapResource> tmp;

        public struct BiomesCellLists
        {
            public List<HexCell> terrainList;
            public List<HexCell> desertList;
            public List<HexCell> tropicList;
            public List<HexCell> winterList;
        }

        BiomesCellLists biomesCellLists;

        MeshFilter meshFilter;
        List<MeshFilter> meshFilters;
        private void Awake()
        {
            manager = FindObjectOfType<PrefabManager>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilters = new List<MeshFilter>();
        }

<<<<<<< Updated upstream
=======
        private int GetPercent(int number, int percent) { return (number * percent) / 100; }

>>>>>>> Stashed changes
        public void CombineMeshes()
        {
            CombineInstance[] combines = new CombineInstance[meshFilters.Count];
            for (int i = 0; i < meshFilters.Count; i++)
            {
                combines[i].mesh = meshFilters[i].sharedMesh;
                combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }
            meshFilter.mesh.CombineMeshes(combines);
            GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
            gameObject.SetActive(true);  
        }

        private MapResource ChooseTreePrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.tree_prefabs.Length - 1);
            return manager.tree_prefabs[prefNum];
        }

        private MapResource ChooseGrassPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.grass_prefabs.Length - 1);
            
            return manager.grass_prefabs[prefNum];
        }

        private MapResource ChooseRockPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.rock_prefabs.Length - 1);
            
            return manager.rock_prefabs[prefNum];
        }
        private MapResource ChooseForestPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.forest_prefabs.Length - 1);

            return manager.forest_prefabs[prefNum];
        }
<<<<<<< Updated upstream
        private int GetTerrainCellsCount(HexGrid grid)
        {
            int count = 0;
            foreach (var cell in grid.cellList)
                if (cell.CellType == HexCell.CellTypes.terrain)
                    count++;
            return count;
=======

        private MapResource ChooseWinterTreePrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.winter_tree_prefabs.Length - 1);

            return manager.winter_tree_prefabs[prefNum];
        }

        private MapResource ChooseWinterGrassPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.winter_grass_prefabs.Length - 1);

            return manager.winter_grass_prefabs[prefNum];
        }

        private void InitSupportLists(HexGrid grid)
        {
            biomesCellLists.terrainList = new List<HexCell>();
            biomesCellLists.desertList = new List<HexCell>();
            biomesCellLists.tropicList = new List<HexCell>();
            biomesCellLists.winterList = new List<HexCell>();
            foreach (var cell in grid.cellList.cells)
            {
                if (cell.Type == CellType.terrain)
                    biomesCellLists.terrainList.Add(cell);
                if (cell.Type == CellType.sand)
                    biomesCellLists.desertList.Add(cell);
                if (cell.Type == CellType.tropic)
                    biomesCellLists.tropicList.Add(cell);
                if (cell.Type == CellType.winter || cell.Type == CellType.taiga)
                    biomesCellLists.winterList.Add(cell);
            }
        }

        private void SetPositionRotationScaling(HexCell cell, int rotation, float scaleBorder, float posY, Func<MapResource> choosePrefab)
        {
            MapResource obj = Instantiate(choosePrefab());
            obj.transform.SetParent(transform);
            Vector3 pos = cell.transform.position;
            Vector3 scale = obj.transform.localScale;

            pos.y += obj.transform.localScale.y * posY;
            obj.transform.position = pos;

            float scaling = UnityEngine.Random.Range((-1) * scaleBorder, scaleBorder);
            scale.x += scaling;
            scale.y += scaling;
            scale.z += scaling;
            obj.transform.localScale = scale;

            obj.transform.rotation = Quaternion.Euler(rotation, UnityEngine.Random.Range(-180, 180), 0f);
            obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
        }

        private int SetTreeCountOnCell(bool isRock, HexCell cell, CellType cellType, int minRockTreeCount, int maxRockTreeCount, int minTreeCount, int maxTreeCount)
        {
            int treeCountOnCell;
            if (isRock)
                treeCountOnCell = UnityEngine.Random.Range(minRockTreeCount, maxRockTreeCount);
            else
            {
                treeCountOnCell = UnityEngine.Random.Range(minTreeCount, maxTreeCount);
                if(cellType != CellType.winter)
                    cell.SetTypeAndTexture(cellType);
            }
            return treeCountOnCell;
>>>>>>> Stashed changes
        }
        private void GenerateRock(HexGrid grid)
        {
            float rndCoeff = 0.8f;
            foreach (var cell in grid.cellList)
                if (cell.CellType == HexCell.CellTypes.terrain || cell.CellType == HexCell.CellTypes.rock)
                {
                    int isRock = UnityEngine.Random.Range(1, 10);
                    var nCells = grid.cellList.GetNeighbours(cell.CellIndex);
                    nCells.Add(cell, 0, 0);
                    bool isNearRock = false;
                    foreach (var nCell in nCells)
                    if (cell.CellType == HexCell.CellTypes.rock)
                    {
                       rndCoeff = 0.7f;
                       isRock = 10;
                       isNearRock = true;
                    }
 
                    if (isRock > 7)
                    {
                        int rockCount = UnityEngine.Random.Range(1, 10);
                        if (isNearRock)
                            rockCount += 5;
                        for (int i = 0; i < rockCount; i++)
                        {
                            MapResource obj = Instantiate(ChooseRockPrefab());
                            
                            meshFilters.Add(obj.GetComponent<MeshFilter>());
                            
                            obj.transform.SetParent(transform);
                            Vector3 pos = cell.transform.position;
                            Quaternion rotation = cell.transform.rotation;
                            Vector3 scaling = cell.transform.localScale;

                            int rotate = UnityEngine.Random.Range(-60, 60);
                            
                            float rndElevation = UnityEngine.Random.Range(0.15f, 0.35f);
                            pos.y += obj.transform.localScale.y * rndElevation;
                            obj.transform.position = pos;

                            rotation.x += rotate;
                            obj.transform.rotation = rotation;

                            scaling.x += UnityEngine.Random.Range(-1.5f, -0.5f);
                            scaling.y += UnityEngine.Random.Range(-1.5f, -0.5f);
                            scaling.z += UnityEngine.Random.Range(-1.5f, -0.5f);
                            obj.transform.localScale += scaling;

                            obj.SetInnerPosition(UnityEngine.Random.Range(-1.0f * rndCoeff, rndCoeff), UnityEngine.Random.Range(-1.0f * rndCoeff, rndCoeff));
                        }

                    }

                }
        }

        public void GenerateTree(HexGrid grid)
        {
<<<<<<< Updated upstream
            int startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            while(grid.cellList[startCell].CellType != HexCell.CellTypes.terrain)
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            int treeChunkCount = UnityEngine.Random.Range(GetTerrainCellsCount(grid) - 100, GetTerrainCellsCount(grid));
            while(treeChunkCount >= 0)
            {
                if(grid.cellList[startCell].CellType == HexCell.CellTypes.terrain)
                {
=======
            
            int startCell = biomesCellLists.terrainList[UnityEngine.Random.Range(0, biomesCellLists.terrainList.Count)].CellIndex;
            int treeChunkCount = GetPercent(biomesCellLists.terrainList.Count, GlobalVariables.generationSettings.standartTreeProcent);

            while(treeChunkCount >= 0)
            {
>>>>>>> Stashed changes
                    CellList neigboursCells = grid.cellList.GetNeighbours(startCell);
                    neigboursCells.Add(grid.cellList[startCell], 0, 0);
                    bool isRock = false;
                    foreach(var cell in neigboursCells)
                        if(cell.CellType == HexCell.CellTypes.rock)
                            isRock = true;
                    foreach(var cell in neigboursCells)
                    {
                        if (cell.CellType == HexCell.CellTypes.terrain)
                        {
<<<<<<< Updated upstream
                            int treeCountOnCell = 0;
                            if (isRock)
                                treeCountOnCell = UnityEngine.Random.Range(0, 1);
                            else
                            {
                                treeCountOnCell = UnityEngine.Random.Range(4,6);
                                cell.CellType = HexCell.CellTypes.dirt;
                            }

                            for(int i = 0; i < treeCountOnCell; i++)
                            {
                                MapResource obj = Instantiate(ChooseTreePrefab());
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

                                //treeList.Add(obj);  ??? начем, а главное захуя 
                                //А затем, что нужно будет как то проверить где деревья находятся при постановке к примеру лесопилки
                                treeChunkCount--;
                            }

                        }
                    }
                }
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
                while (grid.cellList[startCell].CellType != HexCell.CellTypes.terrain)
                    startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            }
        }
        private void GenerateForest(HexGrid grid)
=======
                            int treeCountOnCell = SetTreeCountOnCell(isRock, cell, CellType.forest_dirt, 1, 2, 5, 7);
                            biomesCellLists.terrainList.Remove(cell);
                            for(int i = 0; i < treeCountOnCell; i++)
                            {
                                SetPositionRotationScaling(cell, -90, 0.5f, 0.04f, ChooseTreePrefab);
                            }
                            treeChunkCount--;
                        }
                    }
                startCell = biomesCellLists.terrainList[UnityEngine.Random.Range(0, biomesCellLists.terrainList.Count)].CellIndex;
            }
        }



        public void GenerateTropicTree(HexGrid grid)
        {
            int startCell = biomesCellLists.tropicList[UnityEngine.Random.Range(0, biomesCellLists.tropicList.Count)].CellIndex;
            int treeChunkCount = GetPercent(biomesCellLists.tropicList.Count, GlobalVariables.generationSettings.tropicTreeProcent);
            while (treeChunkCount >= 0)
            {
                    CellList neigboursCells = grid.cellList.GetNeighbours(startCell);
                    neigboursCells.Add(grid.cellList[startCell], 0, 0);
                    bool isRock = false;
                    foreach (var cell in neigboursCells)
                        if (cell.Type == CellType.rock)
                            isRock = true;
                    foreach (var cell in neigboursCells)
                    {
                        if (cell.Type == CellType.tropic)
                        {
                            int treeCountOnCell = SetTreeCountOnCell(isRock, cell, CellType.tropic_dirt, 2, 3, 8, 10);

                            for (int i = 0; i < treeCountOnCell; i++)
                            {
                                SetPositionRotationScaling(cell, 0, 0.5f, 0.04f, ChooseTropicTreePrefab);
                            }
                            treeChunkCount--;
                        }
                    }
                startCell = biomesCellLists.tropicList[UnityEngine.Random.Range(0, biomesCellLists.tropicList.Count)].CellIndex;
            }
        }

        public void GenerateWinterTree(HexGrid grid)
        {
            int startCell = biomesCellLists.winterList[UnityEngine.Random.Range(0, biomesCellLists.winterList.Count)].CellIndex;
            int treeChunkCount = GetPercent(biomesCellLists.winterList.Count, GlobalVariables.generationSettings.winterTreeProcent);
            while (treeChunkCount >= 0)
            {

                    CellList neigboursCells = grid.cellList.GetNeighbours(startCell);
                    neigboursCells.Add(grid.cellList[startCell], 0, 0);
                    bool isRock = false;
                    foreach (var cell in neigboursCells)
                        if (cell.Type == CellType.rock)
                            isRock = true;
                    foreach (var cell in neigboursCells)
                    {
                        if (cell.Type == CellType.winter || cell.Type == CellType.taiga)
                        {
                            int treeCountOnCell = SetTreeCountOnCell(isRock, cell, CellType.winter, 2, 3, 8, 10); ;

                            for (int i = 0; i < treeCountOnCell; i++)
                            {
                                SetPositionRotationScaling(cell, 0, 0.5f, 0.04f, ChooseWinterTreePrefab);
                            }
                            treeChunkCount--;
                        }
                    }
                startCell = biomesCellLists.winterList[UnityEngine.Random.Range(0, biomesCellLists.winterList.Count)].CellIndex;
            }
        }

        private void GenerateForestBush(HexGrid grid)
>>>>>>> Stashed changes
        {
            foreach(var cell in grid.cellList)
            {
                if(cell.CellType == HexCell.CellTypes.dirt)
                {
                    int grassCount = UnityEngine.Random.Range(1, 5);
                    while(grassCount >= 0)
                    {
                        SetPositionRotationScaling(cell, 0, 0.3f, 0.02f, ChooseForestPrefab);
                        grassCount--;
                    }
                }
            }
        }

        private void GenerateBush(HexGrid grid)
        {
<<<<<<< Updated upstream
            foreach(var cell in grid.cellList)
                if(cell.CellType == HexCell.CellTypes.terrain || cell.CellType == HexCell.CellTypes.dirt)
                {
=======
            foreach (var cell in biomesCellLists.terrainList)
            {
>>>>>>> Stashed changes
                    int spawnChance = UnityEngine.Random.Range(0, 10);
                    if (spawnChance > 5)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 5);
                        while(grassCount != 0)
                        {
                            SetPositionRotationScaling(cell, 0, 0.3f, 0.02f, ChooseGrassPrefab);
                            grassCount--;
                        }
                    }
            }

        }

<<<<<<< Updated upstream
        private void GenerateGrass(HexGrid grid)
        {
            foreach(var cell in grid.cellList)
            {
                if(cell.CellType == HexCell.CellTypes.terrain || cell.CellType == HexCell.CellTypes.dirt)
                {

=======
        private void GenerateTropicBush(HexGrid grid)
        {
            foreach (var cell in biomesCellLists.tropicList)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 10);
                    if (spawnChance > 2)
                    {
                        int grassCount = UnityEngine.Random.Range(6, 8);
                        while (grassCount != 0)
                        {
                            SetPositionRotationScaling(cell, 0, 0.3f, 0.02f, ChooseTropicGrassPrefab);
                            grassCount--;
                        }
                    }
                }
        }

        private void GenerateDesertBush(HexGrid grid)
        {
            foreach(var cell in biomesCellLists.desertList)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 11);
                    if(spawnChance > 3)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 6);
                        while(grassCount != 0)
                        {
                            SetPositionRotationScaling(cell, 0, 0.4f, 0.02f, ChooseDesertPrefab);
                            grassCount--;
                        }
                    }
                }
        }

        private void GenerateWinterBush(HexGrid grid)
        {
            foreach (var cell in biomesCellLists.winterList)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 11);
                    if (spawnChance > 4)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 6);
                        while (grassCount != 0)
                        {
                            SetPositionRotationScaling(cell, 0, 0.4f, 0.02f, ChooseWinterGrassPrefab);
                            grassCount--;
                        }
                    }
>>>>>>> Stashed changes
                }
            }
        }

        public void GenerateResource(HexGrid grid)
        {
            InitSupportLists(grid);

            GenerateRock(grid);
            GenerateTree(grid);
            GenerateBush(grid);
            GenerateForest(grid);
        }
    }
}
