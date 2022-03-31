using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Map.MapResources;

public class MainMap: MonoBehaviour
{


    public HexGrid grid;
    public Transform res_prefab;
    MapResource[] resources;

    private void Awake()
    {
        grid = Instantiate(grid);
        grid.transform.SetParent(transform);
        resources = new MapResource[HexMetrics.chunkSizeX * grid.chunkCountX * HexMetrics.chunkSizeZ * grid.chunkCountZ];
        //System.Random rndSeed = new System.Random(300);
        
        foreach (var chunk in grid.chunks)
        {
            foreach (var cell in chunk.cells)
            {
                if(cell.CellColor == cell.terrainColor)
                {
                    MapResource obj = resources[cell.CellIndex] = Instantiate(res_prefab);
                    obj.transform.SetParent(transform);
                    Vector3 pos = cell.transform.position;

                    ResourceGenerator resGen = new ResourceGenerator();


                    pos.y += obj.transform.localScale.y * 0.5f;
                    obj.transform.position = pos;

                    
                    obj.SetInnerPosition(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                }

            }
        }
    }
}
