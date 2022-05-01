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

        MeshFilter meshFilter;
        List<MeshFilter> meshFilters;
        private void Awake()
        {
            manager = FindObjectOfType<PrefabManager>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilters = new List<MeshFilter>();
        }

        private int GetPercent(int number, int percent)
        {
            return (number * percent) / 100;
        }

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

        private MapResource ChooseTropicTreePrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.tropic_tree_prefabs.Length - 1);
            return manager.tropic_tree_prefabs[prefNum];
        }

        private MapResource ChooseGrassPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.grass_prefabs.Length - 1);          
            return manager.grass_prefabs[prefNum];
        }

        private MapResource ChooseTropicGrassPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.tropic_grass_prefab.Length - 1);
            return manager.tropic_grass_prefab[prefNum];
        }

        private MapResource ChooseDesertPrefab()
        {
            if (manager == null)
                Awake();
            int prefNum = UnityEngine.Random.Range(0, manager.desert_prefabs.Length - 1);
            return manager.desert_prefabs[prefNum];
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

        private int GetTerrainCellsCount(HexGrid grid)
        {
            int count = 0;
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.terrain)
                    count++;
            return count;
        }

        private int GetTropicCellsCount(HexGrid grid)
        {
            int count = 0;
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.tropic)
                    count++;
            return count;
        }

        private int GetWinterCellsCount(HexGrid grid)
        {
            int count = 0;
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.taiga || cell.Type == CellType.winter)
                    count++;
            return count;
        }

        private void GenerateRock(HexGrid grid)
        {
            float rndCoeff = 0.8f;
            foreach (var cell in grid.cellList)
                if (cell.Type != CellType.water && cell.Type != CellType.sand)
                {
                    int isRock = UnityEngine.Random.Range(1, 10);
                    var nCells = grid.cellList.GetNeighbours(cell.CellIndex);
                    nCells.Add(cell, 0, 0);
                    bool isNearRock = false;
                    foreach (var nCell in nCells)
                    if (cell.Type == CellType.rock)
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

        public void GenerateTerrainTree(HexGrid grid)
        {
            int startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            while(grid.cellList[startCell].Type != CellType.terrain)
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);

            int treeChunkCount = GetPercent(GetTerrainCellsCount(grid), GlobalVariables.convertor.standartTreeProcent);

            while(treeChunkCount >= 0)
            {
                if(grid.cellList[startCell].Type == CellType.terrain)
                {
                    CellList neigboursCells = grid.cellList.GetNeighbours(startCell);
                    neigboursCells.Add(grid.cellList[startCell], 0, 0);
                    bool isRock = false;
                    foreach(var cell in neigboursCells)
                        if(cell.Type == CellType.rock)
                            isRock = true;
                    foreach(var cell in neigboursCells)
                    {
                        if (cell.Type == CellType.terrain)
                        {
                            int treeCountOnCell = 0;
                            if (isRock)
                                treeCountOnCell = UnityEngine.Random.Range(1, 2);
                            else
                            {
                                treeCountOnCell = UnityEngine.Random.Range(5,7);
                                cell.SetTypeAndTexture(CellType.forest_dirt);
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

                                //treeList.Add(obj);  //??? начем, а главное захуя 
                                //А затем, что нужно будет как то проверить где деревья находятся при постановке к примеру лесопилки
                            }
                            treeChunkCount--;
                        }
                    }
                }
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
                while (grid.cellList[startCell].Type != CellType.terrain)
                    startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            }
        }



        public void GenerateTropicTree(HexGrid grid)
        {
            int startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            while (grid.cellList[startCell].Type != CellType.tropic)
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            int treeChunkCount = GetPercent(GetTropicCellsCount(grid), GlobalVariables.convertor.tropicTreeProcent);
            while (treeChunkCount >= 0)
            {
                if (grid.cellList[startCell].Type == CellType.tropic)
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
                            int treeCountOnCell = 0;
                            if (isRock)
                                treeCountOnCell = UnityEngine.Random.Range(2, 3);
                            else
                            {
                                treeCountOnCell = UnityEngine.Random.Range(8, 10);
                                cell.SetTypeAndTexture(CellType.tropic_dirt);
                            }

                            for (int i = 0; i < treeCountOnCell; i++)
                            {
                                MapResource obj = Instantiate(ChooseTropicTreePrefab());
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

                                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180, 180), 0f);
                                obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
                            }
                            treeChunkCount--;
                        }

                    }
                }
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
                while (grid.cellList[startCell].Type != CellType.tropic)
                    startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            }
        }

        public void GenerateWinterTree(HexGrid grid)
        {
            int startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            while (grid.cellList[startCell].Type != CellType.winter && grid.cellList[startCell].Type != CellType.taiga)
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            int treeChunkCount = GetPercent(GetWinterCellsCount(grid), GlobalVariables.convertor.winterTreeProcent); ;
            while (treeChunkCount >= 0)
            {
                if (grid.cellList[startCell].Type == CellType.winter || grid.cellList[startCell].Type == CellType.taiga)
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
                            int treeCountOnCell = 0;
                            if (isRock)
                                treeCountOnCell = UnityEngine.Random.Range(2, 3);
                            else
                                treeCountOnCell = UnityEngine.Random.Range(8, 10);

                            for (int i = 0; i < treeCountOnCell; i++)
                            {
                                MapResource obj = Instantiate(ChooseWinterTreePrefab());
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

                                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(-180, 180), 0f);
                                obj.SetInnerPosition(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
                            }
                            treeChunkCount--;
                        }

                    }
                }
                startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
                while (grid.cellList[startCell].Type != CellType.winter && grid.cellList[startCell].Type != CellType.taiga)
                    startCell = UnityEngine.Random.Range(0, grid.cellList.Length);
            }
        }

        private void GenerateForestBush(HexGrid grid)
        {
            foreach(var cell in grid.cellList)
            {
                if(cell.Type == CellType.forest_dirt)
                {
                    int grassCount = UnityEngine.Random.Range(1, 5);
                    while(grassCount >= 0)
                    {
                        MapResource obj = Instantiate(ChooseForestPrefab());
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

        private void GenerateTerrainBush(HexGrid grid)
        {
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.terrain || cell.Type == CellType.forest_dirt)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 10);
                    if (spawnChance > 5)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 5);
                        while(grassCount != 0)
                        {
                            MapResource obj = Instantiate(ChooseGrassPrefab());
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

        private void GenerateTropicBush(HexGrid grid)
        {
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.tropic || cell.Type == CellType.tropic_dirt)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 10);
                    if (spawnChance > 5)
                    {
                        int grassCount = UnityEngine.Random.Range(6, 8);
                        while (grassCount != 0)
                        {
                            MapResource obj = Instantiate(ChooseTropicGrassPrefab());
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

        private void GenerateDesertBush(HexGrid grid)
        {
            foreach(var cell in grid.cellList)
                if(cell.Type == CellType.sand)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 11);
                    if(spawnChance > 3)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 6);
                        while(grassCount != 0)
                        {
                            MapResource obj = Instantiate(ChooseDesertPrefab());
                            obj.transform.SetParent(transform);
                            Vector3 pos = cell.transform.position;
                            Vector3 scale = obj.transform.localScale;

                            pos.y += obj.transform.localScale.y * 0.02f;
                            obj.transform.position = pos;

                            float scaling = UnityEngine.Random.Range(-0.4f, 0.4f);
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

        private void GenerateWinterBush(HexGrid grid)
        {
            foreach (var cell in grid.cellList)
                if (cell.Type == CellType.winter || cell.Type == CellType.taiga)
                {
                    int spawnChance = UnityEngine.Random.Range(0, 11);
                    if (spawnChance > 3)
                    {
                        int grassCount = UnityEngine.Random.Range(1, 6);
                        while (grassCount != 0)
                        {
                            MapResource obj = Instantiate(ChooseWinterGrassPrefab());
                            obj.transform.SetParent(transform);
                            Vector3 pos = cell.transform.position;
                            Vector3 scale = obj.transform.localScale;

                            pos.y += obj.transform.localScale.y * 0.02f;
                            obj.transform.position = pos;

                            float scaling = UnityEngine.Random.Range(-0.4f, 0.4f);
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

        public void GenerateResource(HexGrid grid)
        {

            GenerateRock(grid);
            GenerateTerrainTree(grid);
            GenerateTropicTree(grid);
            GenerateWinterTree(grid);

            GenerateForestBush(grid);
            GenerateDesertBush(grid);
            GenerateTropicBush(grid);
            GenerateTerrainBush(grid);
            GenerateWinterBush(grid);
        }
    }
}
