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
        public MapResource treePrefab;
        private void GenerateRock(HexGrid grid, List<MapResource> additionFeatures, System.Random rndSeed)
        {
            //var prManager = new PrefabManager();
            foreach (var cell in grid.cellList)
            {
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

                            int rotate = rndSeed.Next(-180, 180);
                            
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
        }
        public void GenerateResource(HexGrid grid, MapResource[] resources, List<MapResource> additionFeatures, System.Random rndSeed)
        {
            GenerateRock(grid, additionFeatures, rndSeed);
        }
    }
}
