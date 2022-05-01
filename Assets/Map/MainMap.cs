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
        System.Random rnd = new System.Random();
        int rndSeed = rnd.Next(1, 3000000);
        UnityEngine.Random.InitState(rndSeed);
        grid = Instantiate(grid);
        grid.transform.position = Vector3.zero;
        grid.transform.SetParent(transform);

        resGen = FindObjectOfType<ResourceGenerator>();
        resGen.GenerateResource(grid);
    }

}
