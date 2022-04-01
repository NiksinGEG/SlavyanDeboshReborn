using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Map.WorldMap;
using Assets.Map.MapResources;

public class MainMap : MonoBehaviour
{
    public HexGrid grid;

    public MapResource rockPrefab;

    MapResource[] resources;
    List<MapResource> treeList;

    private void Awake()
    {
        grid = Instantiate(grid);
        grid.transform.SetParent(transform);
        treeList = new List<MapResource>();
        resources = new MapResource[HexMetrics.chunkSizeX * grid.chunkCountX * HexMetrics.chunkSizeZ * grid.chunkCountZ];
        System.Random rndSeed = new System.Random(300);

        var resGen = FindObjectOfType(typeof(ResourceGenerator)) as ResourceGenerator;
        resGen.GenerateResource(grid, resources, treeList, rndSeed);
    }
}
