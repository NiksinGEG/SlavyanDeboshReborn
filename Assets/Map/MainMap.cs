using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;

public class MainMap: MonoBehaviour
{
    public HexGrid grid;
    public MapResource res_prefab;
    MapResource[] resources;

    private void Awake()
    {
        grid = Instantiate(grid);
        grid.transform.SetParent(transform);
        resources = new MapResource[HexMetrics.chunkSizeX * grid.chunkCountX * HexMetrics.chunkSizeZ * grid.chunkCountZ];

        foreach (var chunk in grid.chunks)
        {
            foreach (var cell in chunk.cells)
            {
                if(cell.CellColor == cell.terrainColor)
                {
                    MapResource obj = resources[cell.CellIndex] = Instantiate(res_prefab);
                    obj.transform.SetParent(transform);
                    Vector3 pos = cell.transform.position;
                    pos.y += obj.transform.localScale.y * 0.5f;
                    obj.transform.position = pos;

                    obj.SetInnerPosition(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                }

            }
        }
    }
}
