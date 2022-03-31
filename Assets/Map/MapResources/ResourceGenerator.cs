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
        private void GenerateRock(HexGrid grid, List<MapResource> additionFeatures)
        {
            foreach (var chunk in grid.chunks)
            {
                foreach (var cell in chunk.cells)
                {
                    if (cell.CellColor == cell.terrainColor)
                    {
                        MapResource obj = Instantiate(rockPrefab);
                        obj.transform.SetParent(transform);
                        Vector3 pos = cell.transform.position;

                        pos.y += obj.transform.localScale.y * 0.5f;
                        obj.transform.position = pos;


                        obj.SetInnerPosition(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                        
                    }

                }
            }
        }

        public void GenerateResource(HexGrid grid,MapResource[] resources, List<MapResource> additionFeatures, System.Random rndSeed)
        {
            GenerateRock(grid, additionFeatures);
        }
    }
}
